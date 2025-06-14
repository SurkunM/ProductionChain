﻿using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;

public class DeleteProductionTaskHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteProductionTaskHandler> _logger;

    public DeleteProductionTaskHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductionTaskHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ProductionTaskRequest taskRequest)
    {
        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();
        var employeesRepository = _unitOfWork.GetRepository<IEmployeesRepository>();
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();
        var historiesRepository = _unitOfWork.GetRepository<IProductionHistoryRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var success = productionOrdersRepository.SubtractInProgressCount(taskRequest.ProductionOrderId, taskRequest.ProductsCount)
                && productionOrdersRepository.AddCompletedCount(taskRequest.ProductionOrderId, taskRequest.ProductsCount);

            if (!success)
            {
                _logger.LogError("При изменении значений \"InProgress\" и \"Completed\" в производственном заказе произошла ошибка.");

                _unitOfWork.RollbackTransaction();

                return false;
            }

            success = employeesRepository.UpdateEmployeeStatus(taskRequest.EmployeeId, EmployeeStatusType.Available)
                    && productionOrdersRepository.UpdateProductionOrderStatus(taskRequest.ProductionOrderId);

            if (!success)
            {
                _logger.LogError("При изменении статуса сотрудника и производственного заказа произошла ошибка.");

                _unitOfWork.RollbackTransaction();

                return false;
            }

            var task = await tasksRepository.GetByIdAsync(taskRequest.Id);

            if (task is null)
            {
                _logger.LogError("Не удалось найди задачу по переданному id={id}", taskRequest.Id);

                _unitOfWork.RollbackTransaction();

                return false;
            }

            tasksRepository.SetTaskEndTime(taskRequest.Id);
            await historiesRepository.CreateAsync(task.ToHistoryModel());

            tasksRepository.Delete(task);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении задачи из БД произошла ошибка. Транзакция отменена");

            return false;
        }
    }
}

using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class ProductionStage
{
    public int Id { get; set; }

    public required string Name { get; set; }//Пайка, Сборка, Тестирование, Упаковка.

    public virtual ICollection<ProductionTask> Tasks { get; set; } = new List<ProductionTask>();
}

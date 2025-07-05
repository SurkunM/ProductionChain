Пет-проект моделирующий процесс сборки блоков питания на производстве. Система управляет заказами, производственными задачами, сотрудниками и складами компонентов/готовой продукции. Для работы с БД используются шаблоны Repository, Unit of work. Vue.js для клиентской части.

## Основные сущности:

### Базовые модели (ProductionChain.Model):

 - Employee - сотрудники производства,
 - Product - продукция (блоки питания),
 - Order - заказы на производство.

### Рабочий процесс:

 - AssemblyProductionOrders - заказы, переданные в производство (производственная очередь),
 - AssemblyProductionTask - производственные задачи,
 - ProductionHistory - история выполненных задач,
 - AssemblyProductionWarehouse - склад готовой продукции,
 - ComponentsWarehouse - склад компонентов (печатные платы, диодные платы, радиаторы, корпуса),

## Логика работы:
Таблицы Employees, Products, Orders, ComponentsWarehouse считаются, что уже сущ. и инициализируется в ручную.

### 1.Начало производства:
При нажатии "Начать" в клиентской части
  1. Order из состояния "Pending" переходит в "InProgress",
  2. В AssemblyProductionOrders создается "Производственый заказ"

### 2.Создание задач:
При нажатии "Создать задачу":   
  1. Берется необходимое количество компонентов из склада ComponentsWarehouse,
  2. В AssemblyProductionOrders увеличивается строка "Количество продукции в работе",
  3. Обновляются статусы у Employees на "Занят" и в AssemblyProductionOrders на "В работе",
  4. В AssemblyProductionTask создается "Задача", которая содержит информации из "Производственного заказа"
     
### 3.Завершение задачи:
При нажатии "Завершить задачу":
  1. В AssemblyProductionOrders уменьшается строка "Количество продукции в работе" и увеличивается "Количество заверенной продукции",
  2. Обновляются статусы у Employees на "Свободен" и в AssemblyProductionOrders,
  3. Создается "История" в ProductionHistory,
  4. В AssemblyProductionTask удаляется "Задача".

### 4.Завершение заказа:
При нажатии "Завершить производсвтенный заказ":
  1. В Order увеличивается строка "Доступное количество продукции".
  2. Обновляется статус для AssemblyProductionOrders,
  3. Количество собранной продукции добавляется в склад AssemblyProductionWarehouse,
  4. В AssemblyProductionOrders удаляется "Производственный заказ".

### Скриншоты интерфейса
![Screenshot 2025-07-05 103128](https://github.com/user-attachments/assets/1396b686-4b0e-4d3d-9699-691b4020c8de)
![Screenshot 2025-07-05 103229](https://github.com/user-attachments/assets/687c101f-2c6a-4448-8041-a4449cee4322)
![Screenshot 2025-07-05 103108](https://github.com/user-attachments/assets/e31cb91b-fe6f-433f-a8b4-ffae05f0973d)


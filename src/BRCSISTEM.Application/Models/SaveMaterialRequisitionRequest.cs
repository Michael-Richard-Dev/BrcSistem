namespace BRCSISTEM.Application.Models
{
    public sealed class SaveMaterialRequisitionRequest
    {
        public SaveMaterialRequisitionRequest()
        {
            Items = new MaterialRequisitionItemInput[0];
        }

        public string Number { get; set; }

        public string WarehouseCode { get; set; }

        public string MovementDateTime { get; set; }

        public string ReasonName { get; set; }

        public string ActorUserName { get; set; }

        public MaterialRequisitionItemInput[] Items { get; set; }
    }
}

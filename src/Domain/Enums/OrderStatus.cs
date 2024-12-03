using Domain.Utils.Attributes;
using System.ComponentModel;

namespace Domain.Enums
{
    public enum OrderStatus
    {
		[Description("Nenhum")]
        None = -1,
        [Description("Aguardando Pagamento")]
        WaitPayment,
        [Description("Recebido")]
        Received,
        [Description("Em Preparo")]
        InPreparation,
        [Description("Pronto")]
        Ready,
        [Description("Finalizado")]
        Finished,
        [Description("Cancelado")]
        Canceled = -2
    }
}

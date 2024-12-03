using Domain.Utils.Attributes;
using System.ComponentModel;

namespace Domain.Enums
{
    public enum OrderStatus
    {
		[Description("Nenhum")]
        [AlternateValue("NONE")]
        None = -1,
        [Description("Aguardando Pagamento")]
        [AlternateValue("WAITING_PAYMENT")]
        WaitPayment,
        [Description("Recebido")]
        [AlternateValue("RECEIVED")]
        Received,
        [Description("Em Preparo")]
        [AlternateValue("IN_PROGRESS")]
        InPreparation,
        [Description("Pronto")]
        [AlternateValue("READY")]
        Ready,
        [Description("Finalizado")]
        [AlternateValue("FINALIZED")]
        Finished,
        [Description("Cancelado")]
        [AlternateValue("CANCELLED")]
        Canceled = -2
    }
}

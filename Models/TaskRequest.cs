namespace FlowLabourApi.Models
{
    /// <summary>
    /// 任务申请
    /// </summary>
    public class TaskRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TaskId { get; set; }

        public sbyte Agree { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime AgreeDate { get; set; }

        public string? Comment { get; set; }

        public override string ToString()
        {
            return $"{{UserId:{UserId},TaskId:{TaskId},Agree:{Agree},RequestDate:" +
                $"{RequestDate},AgreeDate:{AgreeDate},Comment:{Comment}}}";
        }
    }
}

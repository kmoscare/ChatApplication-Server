using System;
using System.Collections.Generic;

namespace ChatApplication_Server.Models.Tables;

public partial class ChatHistory
{
    public int HistoryId { get; set; }

    public DateTime DatetimeCreated { get; set; }

    public string UserName { get; set; } = null!;

    public string Message { get; set; } = null!;
}

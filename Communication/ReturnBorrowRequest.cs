using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication;
public class ReturnBorrowRequest
{
    private Guid clientId;
    private Guid bookId;

    public ReturnBorrowRequest()
    {
        clientId = Guid.Empty;
        bookId = Guid.Empty;
    }

    public ReturnBorrowRequest(Guid clientId, Guid bookId)
    {
        this.ClientId = clientId;
        this.BookId = bookId;
    }
    public Guid ClientId { get => clientId; set => clientId = value; }
    public Guid BookId { get => bookId; set => bookId = value; }
}

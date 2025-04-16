using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication;

public class SubRequest
{
    private Guid id;

    public NewClientRequest(Guid id)
    {
        this.id = id;
    }

    public Guid Id
    {
        get { return id; }
        set { id = value; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication;

public class SubRequest
{
    private Guid id;

    public SubRequest()
    {
        id = Guid.NewGuid();
    }

    public SubRequest(Guid id)
    {
        this.id = id;
    }

    public Guid Id
    {
        get { return id; }
        set { id = value; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication;

public class SubRequest(Guid id)
{
    private Guid id = id;

    public Guid Id
    {
        get { return id; }
        set { id = value; }
    }
}

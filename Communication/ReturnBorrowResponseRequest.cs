﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class ReturnBorrowResponseRequest
    {
        int result;
        Guid bookId;

        public int Result { get => result; set => result = value; }
        public Guid BookId { get => bookId; set => bookId = value; }

        public ReturnBorrowResponseRequest(int result, Guid bookId)
        {
            this.result = result;
            this.BookId = bookId;
        }
    }
}

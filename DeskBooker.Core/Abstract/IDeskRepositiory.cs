using DeskBooker.Core.Domain;
using System;
using System.Collections.Generic;

namespace DeskBooker.Core.Abstract
{
    public interface IDeskRepositiory
    {
        IEnumerable<Desk> GetAvailableDesks(DateTime date);
    }
}

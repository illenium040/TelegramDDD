﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Domain.Abstractions;

namespace Telegram.Domain.DomainEvents
{
    public record UserCreatedDomainEvent (Guid Id, string UserName) : IDomainEvent;
}

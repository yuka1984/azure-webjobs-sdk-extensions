﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Timers.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Microsoft.Azure.WebJobs.Extensions.Timers.Triggers
{
    internal class TimersTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private static readonly ITimerTriggerArgumentBindingProvider ArgumentBindingProvider =
            new TimerInfoConverterArgumentBindingProvider<TimerInfo>(new AsyncConverter<TimerInfo, TimerInfo>(new IdentityConverter<TimerInfo>()));

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParameterInfo parameter = context.Parameter;
            TimerTriggerAttribute timerTriggerAttribute = parameter.GetCustomAttribute<TimerTriggerAttribute>(inherit: false);

            if (timerTriggerAttribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            IArgumentBinding<TimerInfo> argumentBinding = ArgumentBindingProvider.TryCreate(parameter);
            if (argumentBinding == null)
            {
                throw new InvalidOperationException(string.Format("Can't bind TimerTriggerAttribute to type '{0}'.", parameter.ParameterType));
            }

            ITriggerBinding binding = new TimerTriggerBinding(parameter.Name, parameter.ParameterType, argumentBinding, timerTriggerAttribute);

            return Task.FromResult(binding);
        }
    }
}

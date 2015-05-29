using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil.MefDI.Wcf
{
    public class MefBehaviorAttribute : Attribute, IContractBehavior, IContractBehaviorAttribute
    {
        #region IContractBehaviorAttribute Members

        public Type TargetContract
        {
            get
            {
                return null; //null means we apply to all contracts
            }
        }

        #endregion

        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection parameters)
        {
            return;
        }

        public void ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            return;
        }

        public void ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, DispatchRuntime dispatch)
        {
            Type contractType = description.ContractType;
            dispatch.InstanceProvider = new MefInstanceProvider(contractType);
        }

        public void Validate(ContractDescription description, ServiceEndpoint endpoint)
        {
            return;
        }

        #endregion
    }
}

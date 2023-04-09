using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace UpdateAllContactsAddress
{
    public class UpdateAddress : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext execontext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(execontext.UserId);

            Guid accountid = this.acccountguid.Get(context).Id;

            Entity account = new Entity("account");
            ColumnSet attributes = new ColumnSet(new string[] { "address1_line1", "address1_line2", "address1_line3" });
            account = service.Retrieve(account.LogicalName, accountid, attributes);

            //read related contacts

            QueryExpression query = new QueryExpression("contact");
            query.Criteria.AddCondition(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, accountid));

            DataCollection<Entity> contactsret = service.RetrieveMultiple(query).Entities;

            foreach (Entity entity in contactsret)
            {
                entity["address1_line1"] = account["address1_line1"];
                entity["address1_line2"] = account["address1_line2"];
                entity["address1_line3"] = account["address1_line3"];

                service.Update(entity);
            }
        }

        [RequiredArgument]
        [ReferenceTarget("account")]
        [Input("accountguid")]
        public InArgument<EntityReference> acccountguid { get; set; }
    }
}

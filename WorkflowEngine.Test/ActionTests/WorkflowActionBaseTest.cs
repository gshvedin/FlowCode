using System;
using System.Xml.Linq;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Actions;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace WorkflowEngine.Test.Action
{
    public class WorkflowActionBaseTest
    {
        #region Name
        [Theory, AutoData]
        internal void Name_SetItem_ReturnAttributeValue(WorkflowActionTest workflowAction, string nameVal)
        {
            // Arrange
            workflowAction.Item = new XElement("Test", new XAttribute("name", nameVal));

            // Act
            string res = workflowAction.Name;

            // Assert
            Assert.Equal(res, nameVal);
        }

        [Theory, AutoData]
        internal void Name_SetItem_ReturnDefaultValue(WorkflowActionTest workflowAction)
        {
            // Arrange

            // Act
            string res = workflowAction.Name;

            // Assert
            Assert.Equal(res, workflowAction.GetType().Name);
        }
        #endregion

        #region SkipExecute
        [Theory, AutoData]
        internal void SkipExecute_CheckUserTaskAction_ReturnTrue(UserTaskAction userTaskAction)
        {
            // Arrange

            // Act
            bool skipExecute = userTaskAction.SkipExecute;

            // Assert
            Assert.True(skipExecute);
        }

        [Theory, AutoData]
        internal void SkipExecute_CheckFinishProcess_ReturnTrue(FinishProcess userTaskAction)
        {
            // Arrange

            // Act
            bool skipExecute = userTaskAction.SkipExecute;

            // Assert
            Assert.True(skipExecute);
        }

        [Theory, AutoData]
        internal void SkipExecute_CheckStartProcess_ReturnFalse(StartProcess userTaskAction)
        {
            // Arrange

            // Act
            bool skipExecute = userTaskAction.SkipExecute;

            // Assert
            Assert.False(skipExecute);
        }
        #endregion

        #region Id
        [Theory, AutoData]
        internal void Name_SetItem_ReturnAttributeId(WorkflowActionTest workflowAction, Guid testGuid)
        {
            // Arrange
            workflowAction.Item = new XElement("Test", new XAttribute("id", testGuid));

            // Act
            Guid res = workflowAction.Id;

            // Assert
            Assert.Equal(testGuid, res);
        }

        [Theory, AutoData]
        internal void Name_SetItem_ReturnDefaultId(WorkflowActionTest workflowAction)
        {
            // Arrange
            workflowAction.Item = new XElement("Test", string.Empty);

            // Act
            Guid res = workflowAction.Id;

            // Assert
            Assert.Equal(Guid.Empty, res);
        }
        #endregion

        #region ContextData
        [Theory, AutoData]
        internal void ContextData_InstanceSet_InstanceIsEqual(
                                                    Mock<IInstance> mockInstance,
                                                    XElement item)
        {
            // Arrange
            // Act
            WorkflowActionTest workflowActionTest = new WorkflowActionTest(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Assert
            Assert.Equal(mockInstance.Object, workflowActionTest.CurrentInstance);
        }
        #endregion

        #region Audit
        [Theory, AutoData]
        internal void Audit_AddAudit_SuccessfulCheckSettingData(
                                                Mock<IInstance> mockInstance,
                                                Mock<IContextData> mockContextData,
                                                IList<WorkflowAuditItem> list,
                                                string info,
                                                WorkflowAuditState state,
                                                Guid testGuid,
                                                string nameVal)
        {
            // Arrange
            int firstListLengs = list.Count;

            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(c => c.GetCurrentRequestId())
                           .Returns(testGuid);
            mockInstance.Setup(m => m.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("id", testGuid), new XAttribute("name", nameVal));
            WorkflowActionTest workflowAction = new WorkflowActionTest(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            workflowAction.Audit(info, state);

            // Assert
            Assert.Equal(firstListLengs + 1, list.Count);
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == nameVal);
            Assert.Equal(testGuid, workflow.NodeId);
            Assert.Equal(nameVal, workflow.NodeName);
            Assert.Equal(info, workflow.Info);
            Assert.Equal(testGuid, workflow.RequestId);
            Assert.Equal(info, workflow.Info);
            Assert.Equal(state, workflow.State);
            Assert.NotEqual(default(DateTime), workflow.Timestamp);
        }
        #endregion

        internal class WorkflowActionTest : WorkflowActionBase
        {
            public WorkflowActionTest(XElement item)
            {
                Item = item;
            }

            public override Task ExecuteAsync()
            {
                throw new NotImplementedException();
            }
        }
    }
}

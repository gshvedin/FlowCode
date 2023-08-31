using System;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Test.MiscTest
{
    public class WorkflowExceptionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_SetMessage_SuccessSetting(string message)
        {
            // Arrange

            // Act
            WorkflowException exception = new WorkflowException(message);

            // Assert
            Assert.NotEqual(exception, default(WorkflowException));
            Assert.Equal(message, exception.Message);
        }

        [Theory, AutoData]
        public void Constructor_SetMessageAndCode_SuccessSetting(string message, WorkflowExceptionCode code)
        {
            // Arrange

            // Act
            WorkflowException exception = new WorkflowException(message, code);

            // Assert
            Assert.NotEqual(exception, default(WorkflowException));
            Assert.Equal(message, exception.Message);
            Assert.Equal(code, exception.Code);
        }

        [Theory, AutoData]
        public void Constructor_SetMessageAndInnerException_SuccessSetting(string message, Exception innerException)
        {
            // Arrange

            // Act
            WorkflowException exception = new WorkflowException(message, innerException);

            // Assert
            Assert.NotEqual(exception, default(WorkflowException));
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }
        #endregion
    }
}

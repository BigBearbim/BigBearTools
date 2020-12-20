using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace BigBearTools
{
    public class Transaction : Autodesk.Revit.DB.Transaction
    {
        public Transaction(Document document, string name) : base(document, name)
        {
            FailureHandlingOptions failureHandlingOptions = this.GetFailureHandlingOptions();
            FailureHandler failureHandler = new FailureHandler();
            failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
            failureHandlingOptions.SetClearAfterRollback(true);
            this.SetFailureHandlingOptions(failureHandlingOptions);
        }

        public Transaction(Document document) : base(document)
        {
            FailureHandlingOptions failureHandlingOptions = this.GetFailureHandlingOptions();
            FailureHandler failureHandler = new FailureHandler();
            failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
            failureHandlingOptions.SetClearAfterRollback(true);
            this.SetFailureHandlingOptions(failureHandlingOptions);
        }
    }
}

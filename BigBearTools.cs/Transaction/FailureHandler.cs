using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public class FailureHandler : IFailuresPreprocessor
    {
        //收集错误的信息
        public string ErrorMessage { set; get; }
        //未知。。。
        public string ErrorSeverity { set; get; }

        public FailureHandler()
        {
            ErrorMessage = "";
            ErrorSeverity = "";
        }
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            //收集所有的故障访问器
            IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
            foreach (FailureMessageAccessor failureMessageAccessor in failureMessages)
            {
                FailureDefinitionId id = failureMessageAccessor.GetFailureDefinitionId();

                try
                {
                    ErrorMessage = failureMessageAccessor.GetDescriptionText();
                }
                catch
                {
                    ErrorMessage = "Unknown Error";
                }

                try
                {
                    FailureSeverity failureSeverity = failureMessageAccessor.GetSeverity();
                    ErrorSeverity = failureSeverity.ToString();


                    if (failureSeverity == FailureSeverity.Warning)
                    {
                        failuresAccessor.DeleteWarning(failureMessageAccessor);

                        if (!string.IsNullOrEmpty(ErrorMessage))
                        {
                            return FailureProcessingResult.ProceedWithCommit;
                        }
                        else
                        {
                            return FailureProcessingResult.Continue;
                        }

                    }
                    else
                    {
                        if (ErrorMessage.Contains("幕墙嵌板将由系统嵌板替换"))
                        {
                            failureMessageAccessor.SetCurrentResolutionType(FailureResolutionType.Default);
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }
                        if (ErrorMessage.Contains("无法生成类型"))
                        {
                            //IList<ElementId> id_list = failureMessageAccessor.GetFailingElementIds().ToList();
                            failureMessageAccessor.SetCurrentResolutionType(FailureResolutionType.DeleteElements);
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        if (ErrorMessage == "无法在楼板草图中添加方向曲线。")
                        {
                            //弹出这个错误时，确定按钮是灰色的，只能点取消，因此下面这句要注释掉
                            //failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithRollBack;
                        }

                        if (ErrorMessage == "无法剪切连接的图元。")
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        if (ErrorMessage.Contains("连接无效的反方向"))
                        {
                            return FailureProcessingResult.ProceedWithRollBack;
                        }

                        if (ErrorMessage.Contains("丢失的幕墙嵌板"))
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        if (ErrorMessage.Contains("不能剪切任何对象"))
                        {
                            //墙上如果有门，断开墙体后会弹出上述提示，应点击“删除”，以删除多余的门窗。
                            IList<ElementId> id_list = failureMessageAccessor.GetFailingElementIds().ToList();
                            foreach (ElementId idd in failureMessageAccessor.GetFailingElementIds().ToList())
                            {
                                if (failuresAccessor.GetDocument().GetElement(idd) is Wall)
                                    id_list.Remove(idd);
                            }

                            failuresAccessor.DeleteElements(id_list);
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        if (ErrorMessage.Contains("无法使图元保持连接"))
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        if (ErrorMessage.Contains("约束"))
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        if (ErrorMessage == "不能进行拉伸。" || ErrorMessage == "线太短。" || ErrorMessage == "立面轮廓草图无效。")
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        if (ErrorMessage.Contains("不满足限制条件"))
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        if (ErrorMessage.Contains("不能满足定义高亮显示的图元的绘制限制条件"))
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }
                        if (ErrorMessage.Contains("高亮显示的图元中有一个循环参照链"))
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.ProceedWithCommit;
                        }
                        else
                        {
                            failuresAccessor.ResolveFailure(failureMessageAccessor);
                            return FailureProcessingResult.Continue;
                        }
                    }
                }
                catch
                {
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
}

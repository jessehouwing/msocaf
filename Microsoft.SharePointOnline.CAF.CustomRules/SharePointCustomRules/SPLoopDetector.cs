namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;

    internal class SPLoopDetector
    {
        private List<Instruction> m_ListInstructionsWithinLoop = new List<Instruction>();

        public List<Instruction> CheckAndGetInstructionsWithinLoopIfExists(Method method)
        {
            string str2;
            string str = string.Empty;
            int iInstructionListStartIndex = 0;
            bool bIsFirstInstruction = true;
            bool flag2 = false;
            try
            {
                for (short i = 0; i < method.Instructions.Count; i = (short) (i + 1))
                {
                    if ((method.Instructions[i].OpCode == OpCode.Br_S) || (method.Instructions[i].OpCode == OpCode.Br))
                    {
                        str = Convert.ToString(method.Instructions[i + 1].Offset);
                        this.ForwardAndFillInstructionsTillFirstJump(method, ref i, ref iInstructionListStartIndex, ref bIsFirstInstruction);
                        this.ForwardAndFillInstructionsTillFirstBrTrue(method, ref i);
                        if (method.Instructions[i].OpCode.ToString().Contains(OpCode.Brtrue.ToString()) && ((method.Instructions[i].Value != null) && method.Instructions[i].Value.ToString().Equals(str)))
                        {
                            flag2 = true;
                        }
                        if (!((iInstructionListStartIndex < 0) || flag2))
                        {
                            this.m_ListInstructionsWithinLoop.RemoveRange(iInstructionListStartIndex, this.m_ListInstructionsWithinLoop.Count - iInstructionListStartIndex);
                        }
                        bIsFirstInstruction = true;
                        flag2 = false;
                    }
                    else if (method.Instructions[i].OpCode.ToString().Contains(OpCode.Brtrue.ToString()))
                    {
                        this.RewindTillBackJump(method, i);
                    }
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "CheckAndGetInstructionsWithinLoopIfExists() - " + exception.Message);
            }
            catch (NullReferenceException exception2)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "CheckAndGetInstructionsWithinLoopIfExists() - " + exception2.Message);
            }
            catch (Exception exception3)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "CheckAndGetInstructionsWithinLoopIfExists() - " + exception3.Message);
            }
            return this.m_ListInstructionsWithinLoop;
        }

        private void ForwardAndFillInstructionsTillFirstBrTrue(Method method, ref short nIndex)
        {
            try
            {
                while (!method.Instructions[nIndex].OpCode.ToString().Contains(OpCode.Brtrue.ToString()))
                {
                    if (method.Instructions[nIndex].Value != null)
                    {
                        this.m_ListInstructionsWithinLoop.Add(method.Instructions[nIndex]);
                    }
                    nIndex = (short) (nIndex + 1);
                    if (nIndex >= method.Instructions.Count)
                    {
                        nIndex = (short) (nIndex - 1);
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "ForwardAndFillInstructionsTillFirstBrTrue() - " + exception.Message);
            }
        }

        private void ForwardAndFillInstructionsTillFirstJump(Method method, ref short nIndex, ref int iInstructionListStartIndex, ref bool bIsFirstInstruction)
        {
            string str2;
            try
            {
                string str = method.Instructions[nIndex].Value.ToString();
                while (!method.Instructions[nIndex].Offset.ToString().Equals(str))
                {
                    if (method.Instructions[nIndex].Value != null)
                    {
                        this.m_ListInstructionsWithinLoop.Add(method.Instructions[nIndex]);
                        if (bIsFirstInstruction)
                        {
                            iInstructionListStartIndex = this.m_ListInstructionsWithinLoop.Count - 1;
                            bIsFirstInstruction = false;
                        }
                    }
                    nIndex = (short) (nIndex + 1);
                    if (nIndex >= method.Instructions.Count)
                    {
                        nIndex = (short) (nIndex - 1);
                        return;
                    }
                }
            }
            catch (NullReferenceException exception)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "ForwardAndFillInstructionsTillFirstJump() - " + exception.Message);
            }
            catch (Exception exception2)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "ForwardAndFillInstructionsTillFirstJump() - " + exception2.Message);
            }
        }

        private void RewindTillBackJump(Method method, short nIndex)
        {
            string str;
            short num = nIndex;
            bool flag = true;
            int index = 0;
            try
            {
                int num3 = Convert.ToInt32(method.Instructions[num].Value);
                while (method.Instructions[num].Offset != num3)
                {
                    if (method.Instructions[num].Value != null)
                    {
                        this.m_ListInstructionsWithinLoop.Add(method.Instructions[num]);
                        if (flag)
                        {
                            index = this.m_ListInstructionsWithinLoop.Count - 1;
                            flag = false;
                        }
                    }
                    num = (short) (num - 1);
                    if (num < 0)
                    {
                        num = (short) (num + 1);
                        break;
                    }
                }
                if (!method.Instructions[num].OpCode.Equals(OpCode.Nop))
                {
                    this.m_ListInstructionsWithinLoop.RemoveRange(index, this.m_ListInstructionsWithinLoop.Count - index);
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "RewindTillBackJump () - " + exception.Message);
            }
            catch (Exception exception2)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "RewindTillBackJump () - " + exception2.Message);
            }
        }
    }
}


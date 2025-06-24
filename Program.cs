using static System.Text.RegularExpressions.Regex;

namespace risc_v_Assembler
{
    internal class Program
    {
        static Assembler.Program m_prog = new();
        static List<string> curr_text_dir = [];
        static List<string> curr_data_dir = [];
        static List<string> curr_insts = [];
        static void Assemble(List<KeyValuePair<string, int>> addresses)
        {
            foreach (KeyValuePair<string, int> address in addresses)
            {
                for (int i = 0; i < curr_text_dir.Count; i++)
                {
                    curr_text_dir[i] = Replace(curr_text_dir[i], $@"\b{Escape(address.Key)}\b", address.Value.ToString());
                }
            }
            Assembler.Assembler assembler = new();
            Assembler.Program? program = assembler.ASSEMBLE(curr_text_dir);
            if (program.HasValue)
            {
                m_prog = program.Value;
                curr_insts = assembler.GetInstsAsText(m_prog);
            }
            else
            {
                m_prog.instructions.Clear();
                m_prog.mc.Clear();
                curr_insts.Clear();
            }
            if (assembler.lblINVINST)
            {
                Shartilities.Log(Shartilities.LogType.ERROR, $"error in {"lblINVINST".ToLower()}\n", 1);
            }
            if (assembler.lblinvlabel)
            {
                Shartilities.Log(Shartilities.LogType.ERROR, $"error in {"lblinvlabel".ToLower()}\n", 1);
            }
            if (assembler.lblmultlabels)
            {
                Shartilities.Log(Shartilities.LogType.ERROR, $"error in {"lblmultlabels".ToLower()}\n", 1);
            }
        }
        static void HandleCommand(string[] args)
        {
            Shartilities.ShiftArgs(ref args, out string source_filepath);
            Shartilities.ShiftArgs(ref args, out string MC_filepath);
            Shartilities.ShiftArgs(ref args, out string DM_filepath);
            Shartilities.ShiftArgs(ref args, out string IM_INIT_filepath);
            Shartilities.ShiftArgs(ref args, out string DM_INIT_filepath);
            Shartilities.ShiftArgs(ref args, out string IM_MIF_filepath);
            Shartilities.ShiftArgs(ref args, out string DM_MIF_filepath);

            List<string> DM_INIT = [], DM = [];
            if (source_filepath != null)
            {
                List<string> src = File.ReadAllLines(source_filepath).ToList();
                LibUtils.LibUtils.clean_comments(ref src);
                (List<string> data_dir, List<string> text_dir) = LibUtils.LibUtils.Get_directives(src);
                curr_data_dir = data_dir;
                curr_text_dir = text_dir;
                (List<string> DM_INIT1, List<string> DM1, List<KeyValuePair<string, int>> addresses) = LibUtils.LibUtils.assemble_data_dir(curr_data_dir);
                DM_INIT = DM_INIT1;
                DM = DM1;
                Assemble(addresses);
            }

            if (MC_filepath != null)
                File.WriteAllLines(MC_filepath, m_prog.mc);
            if (DM_filepath != null)
                File.WriteAllLines(DM_filepath, DM);

            if (IM_INIT_filepath != null)
            {
                List<string> IM_INIT = LibUtils.LibUtils.get_IM_INIT(m_prog.mc, curr_insts);
                File.WriteAllLines(IM_INIT_filepath, IM_INIT);
            }
            if (DM_INIT_filepath != null)
                File.WriteAllLines(DM_INIT_filepath, DM_INIT);

            if (IM_MIF_filepath != null)
                File.WriteAllText(IM_MIF_filepath, LibUtils.LibUtils.GetIMMIF(m_prog.mc, 32, 2048, 2).ToString());
            if (DM_MIF_filepath != null)
                File.WriteAllText(DM_MIF_filepath, LibUtils.LibUtils.GetDMMIF(DM, 32, 4096, 10).ToString());
        }
        static void Main(string[] args)
        {
            if (args.Length != 7)
                Shartilities.Log(Shartilities.LogType.ERROR, $"missing arguments\n", 1);
            HandleCommand(args);
        }
    }
}

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
            Assembler.Program program = assembler.AssembleProgram(curr_text_dir);
            m_prog = program;
            curr_insts = LibUtils.LibUtils.GetInstsAsText(m_prog);
        }
        static void Usage()
        {
            Console.WriteLine($"Usage: assembler [options] <source_file>\n");
            Console.WriteLine($"Options:");
            Console.WriteLine($"  --im-init <file>     Path to output instruction memory initialization file");
            Console.WriteLine($"  --dm-init <file>     Path to output data memory initialization file");
            Console.WriteLine($"  -mc <file>           Path to output machine code file");
            Console.WriteLine($"  -dm <file>           Path to output data memory file");
            Console.WriteLine($"  --im-mif <file>      Path to output instruction memory .mif file");
            Console.WriteLine($"  --dm-mif <file>      Path to output data memory .mif file");
            Console.WriteLine();
            Console.WriteLine($"Arguments:");
            Console.WriteLine($"  <source_file>        Assembly source file to be assembled");
            Console.WriteLine();
            Console.WriteLine($"Example:");
            Console.WriteLine($"  {Environment.ProcessPath} --im-init init_im.txt --dm-init init_dm.txt -mc out.mc -dm out.dm source.asm");
        }
        static void Main(string[] args)
        {
            string? source_filepath = null;
            string? IM_INIT_filepath = null;
            string? DM_INIT_filepath = null;
            string? MC_filepath = null;
            string? DM_filepath = null;
            string? IM_MIF_filepath = null;
            string? DM_MIF_filepath = null; 
            while (args.Length > 0)
            {
                Shartilities.ShiftArgs(ref args, out string arg);
                if (arg == "--im-init")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_IM_INIT_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument instruction memory init file path\n", 1);
                    IM_INIT_filepath = temp_IM_INIT_filepath;
                }
                else if (arg == "--dm-init")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_DM_INIT_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument data memory init file path\n", 1);
                    DM_INIT_filepath = temp_DM_INIT_filepath;
                }
                else if (arg == "-mc")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_MC_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument machine code file path\n", 1);
                    MC_filepath = temp_MC_filepath;
                }
                else if (arg == "-dm")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_DM_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument data memory file path\n", 1);
                    DM_filepath = temp_DM_filepath;
                }
                else if (arg == "--im-mif")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_IM_MIF_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument instruction memory mif file path\n", 1);
                    IM_MIF_filepath = temp_IM_MIF_filepath;
                }
                else if (arg == "--dm-mif")
                {
                    if (!Shartilities.ShiftArgs(ref args, out string temp_DM_MIF_filepath))
                        Shartilities.Log(Shartilities.LogType.ERROR, $"Missing argument data memory mif file path\n", 1);
                    DM_MIF_filepath = temp_DM_MIF_filepath;
                }
                else
                {
                    if (source_filepath == null)
                    {
                        source_filepath = arg;
                    }
                    else
                    {
                        Shartilities.Log(Shartilities.LogType.ERROR, $"more than one source file path was provided\n", 1);
                    }
                }
            }
            if (source_filepath == null)
            {
                Shartilities.Log(Shartilities.LogType.ERROR, $"source file path was not provided\n", 1);
                Usage();
                return;
            }

            List<string> DM_INIT = [], DM = [];
            List<string> src = [.. File.ReadAllLines(source_filepath)];
            LibUtils.LibUtils.clean_comments(ref src);
            (List<string> data_dir, List<string> text_dir) = LibUtils.LibUtils.Get_directives(src);
            curr_data_dir = data_dir;
            curr_text_dir = text_dir;
            (List<string> DM_INIT1, List<string> DM1, List<KeyValuePair<string, int>> addresses) = LibUtils.LibUtils.assemble_data_dir(curr_data_dir);
            DM_INIT = DM_INIT1;
            DM = DM1;
            Assemble(addresses);

            List<string> IM_INIT = LibUtils.LibUtils.get_IM_INIT(m_prog.mc, curr_insts);


            if (IM_INIT_filepath != null)
                File.WriteAllLines(IM_INIT_filepath, IM_INIT);
            if (DM_INIT_filepath != null)
                File.WriteAllLines(DM_INIT_filepath, DM_INIT);

            if (MC_filepath != null)
                File.WriteAllLines(MC_filepath, m_prog.mc);
            if (DM_filepath != null)
                File.WriteAllLines(DM_filepath, DM);

            if (IM_MIF_filepath != null)
                File.WriteAllText(IM_MIF_filepath, LibUtils.LibUtils.GetIMMIF(m_prog.mc, 32, 2048, 2).ToString());
            if (DM_MIF_filepath != null)
                File.WriteAllText(DM_MIF_filepath, LibUtils.LibUtils.GetDMMIF(DM, 32, 4096, 10).ToString());
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Text;

namespace risc_v_Assembler
{
    internal class Program
    {
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
            Console.WriteLine($"  -log                 enable loging instruction's attributes while assembling");
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
            bool LOG_INST_FLAG = false;
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
                else if (arg == "-log")
                {
                    LOG_INST_FLAG = true;
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

            LibUtils.Program p = Assembler.Assembler.AssembleProgram(source_filepath, LOG_INST_FLAG);

            StringBuilder IM_INIT = LibUtils.GetIM_INIT(p.MachineCodes, p.Instructions);
            StringBuilder DM_INIT = LibUtils.GetDM_INIT(p.DataMemoryValues);
            if (IM_INIT_filepath != null)
            {
                File.WriteAllText(IM_INIT_filepath, IM_INIT.ToString());
                Shartilities.Log(Shartilities.LogType.INFO, $"Generated {IM_INIT_filepath}\n");
            }
            if (DM_INIT_filepath != null)
            {
                File.WriteAllText(DM_INIT_filepath, DM_INIT.ToString());            
                Shartilities.Log(Shartilities.LogType.INFO, $"Generated {DM_INIT_filepath}\n");
            }

            if (MC_filepath != null)
            {
                List<string> IM = LibUtils.GetIM(p.MachineCodes);
                File.WriteAllLines(MC_filepath, IM);
                Shartilities.Log(Shartilities.LogType.INFO, $"Generated {MC_filepath}\n");
            }

            if (DM_filepath != null)
            {
                List<string> DM = LibUtils.ParseDataMemoryValues(p.DataMemoryValues);
                File.WriteAllLines(DM_filepath, DM);
                Shartilities.Log(Shartilities.LogType.INFO, $"Generated {DM_filepath}\n");
            }

            if (IM_MIF_filepath != null)
            {
                //File.WriteAllText(IM_MIF_filepath, LibUtils.LibUtils.GetIMMIF(p.MachineCodes, 32, 2048, 2).ToString());
            }

            if (DM_MIF_filepath != null)
            {
                File.WriteAllText(DM_MIF_filepath, LibUtils.GetDMMIF(p.DataMemoryValues).ToString());
                Shartilities.Log(Shartilities.LogType.INFO, $"Generated {DM_MIF_filepath}\n");
            }
        }
    }
}

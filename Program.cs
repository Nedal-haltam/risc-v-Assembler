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

            string src = File.ReadAllText(source_filepath);
            Assembler.Program p = Assembler.Assembler.AssembleProgram(src);

            List<string> IM_INIT = LibUtils.LibUtils.GetIM_INIT(p.MachineCodes, p.Instructions);
            List<string> DM_INIT = LibUtils.LibUtils.GetDM_INIT(p.DataMemoryValues);

            if (IM_INIT_filepath != null)
                File.WriteAllLines(IM_INIT_filepath, IM_INIT);
            if (DM_INIT_filepath != null)
                File.WriteAllLines(DM_INIT_filepath, DM_INIT);

            if (MC_filepath != null)
                File.WriteAllLines(MC_filepath, p.MachineCodes);
            if (DM_filepath != null)
                File.WriteAllLines(DM_filepath, p.DataMemoryValues);

            if (IM_MIF_filepath != null)
            {
                Shartilities.TODO($"generating mif files is unsupported for now");
                //File.WriteAllText(IM_MIF_filepath, LibUtils.LibUtils.GetIMMIF(p.MachineCodes, 32, 2048, 2).ToString());
            }
            if (DM_MIF_filepath != null)
            {
                Shartilities.TODO($"generating mif files is unsupported for now");
                //File.WriteAllText(DM_MIF_filepath, LibUtils.LibUtils.GetDMMIF(p.DataMemoryValues, 32, 4096, 10).ToString());
            }
        }
    }
}

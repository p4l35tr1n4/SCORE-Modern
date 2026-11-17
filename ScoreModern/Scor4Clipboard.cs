using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ScoreModern {
    public sealed partial class Editor {
        const string Scor4CutFile = "!!CUT.TMP";
        const string Scor4PastePrompt = "Enter new Staff#, Left pos, Right pos, [steps up-down -- or use mouse] or <esc>";
        sealed class Scor4CutItems {
            internal int Staff, UpperStaff;
            internal float Left, Right;
            internal readonly List<Item> Items = new List<Item>();
        }
        Scor4CutItems pendingScor4Paste;

        bool TryScor4Clipboard(string verb,string rest) {
            if(!scor4Interface)return false;
            if(verb=="XX"&&mode==ScreenMode.Edit){
                RequireNoLetterArguments(verb,rest);
                if(canvas.Selected==null)return true;
                Item item=NewEditedRecord(canvas.Selected);
                var group=new Scor4CutItems {Staff=item.Staff,UpperStaff=item.Staff,Left=item.P(3),Right=(float)((double)item.P(3)+10)};
                group.Items.Add(item);
                WriteScor4CutFile(group);
                ExitEdit(false);
                return true;
            }
            if((verb=="PS"||verb=="PST")&&mode==ScreenMode.Main){
                RequireNoLetterArguments(verb,rest);
                BeginScor4Paste();
                return true;
            }
            if(verb=="CT"&&mode==ScreenMode.Main){
                StartMainGroup(verb,rest);
                Message("Enter Staff#, Left pos., Right pos., [Code#, Upper Staff# -- or use mouse.]");
                return true;
            }
            return false;
        }

        void SubmitScor4CutCode(string text) {
            var code=LayoutNumbers(text);
            if(code.Length!=1)throw new InvalidDataException("Type Code number. 0 = all.");
            groupRange[3]=GroupInteger(code[0],0,99);
            var group=new Scor4CutItems {Staff=(int)groupRange[0],UpperStaff=(int)groupRange[4],Left=groupRange[1],Right=groupRange[2]};
            group.Items.AddRange(score.Items.Where(InMainGroup));
            if(group.Items.Count==0){ClearMainGroup();Message("No items in the selected range.");return;}
            WriteScor4CutFile(group);
            ClearMainGroup();
            Message("");
        }

        void WriteScor4CutFile(Scor4CutItems group) {
            GroupInteger(group.Staff,1,32);
            GroupInteger(group.UpperStaff,group.Staff,32);
            if(group.Items.Count>1420||!(group.Right>group.Left)||Single.IsInfinity(group.Right)||Single.IsInfinity(group.Left))
                throw new InvalidDataException("Invalid SCOR4 cut bounds or item count.");
            using(var stream=new MemoryStream())
            using(var writer=new BinaryWriter(stream)){
                writer.Write((ushort)group.Items.Count);
                writer.Write((ushort)group.Staff);
                writer.Write(group.Left);
                writer.Write(group.Right);
                writer.Write((ushort)group.UpperStaff);
                foreach(var item in group.Items){
                    writer.Write((ushort)item.Count);
                    writer.Write(item.Bytes);
                }
                writer.Flush();
                var path=NativeOutputLocation.RequireWritableDestination(source,Path.GetFullPath(Scor4CutFile));
                ScoreFileStorage.Write(path,stream.ToArray());
            }
        }

        static Scor4CutItems ReadScor4CutFile(string path) {
            using(var reader=new BinaryReader(File.OpenRead(path))){
                int count=reader.ReadUInt16();
                var group=new Scor4CutItems {Staff=reader.ReadUInt16(),Left=reader.ReadSingle(),Right=reader.ReadSingle()};
                GroupInteger(group.Staff,1,32);
                if(count<1||count>1420||!(group.Right>group.Left)||Single.IsInfinity(group.Right)||Single.IsInfinity(group.Left))
                    throw new InvalidDataException("Invalid SCOR4 cut bounds or item count.");
                group.UpperStaff=reader.ReadUInt16();
                GroupInteger(group.UpperStaff,group.Staff,32);
                for(int i=0;i<count;i++){
                    int parameters=reader.ReadUInt16();
                    if(parameters<3||parameters>(reader.BaseStream.Length-reader.BaseStream.Position)/4-1)
                        throw new InvalidDataException("Invalid SCOR4 cut item size.");
                    var bytes=reader.ReadBytes((parameters+1)*4);
                    var item=new Item(bytes);
                    if(BitConverter.ToSingle(bytes,0)!=parameters||item.Staff<group.Staff||item.Staff>group.UpperStaff)
                        throw new InvalidDataException("Invalid SCOR4 cut item header.");
                    group.Items.Add(item);
                }
                if(reader.BaseStream.Position!=reader.BaseStream.Length)
                    throw new InvalidDataException("Unexpected data after the SCOR4 cut items.");
                return group;
            }
        }

        void BeginScor4Paste() {
            var path=Path.GetFullPath(Scor4CutFile);
            if(!File.Exists(path)){Message("File not found - "+Scor4CutFile);return;}
            pendingScor4Paste=ReadScor4CutFile(path);
            groupCommand="PST";
            groupRange=null;
            Message(Scor4PastePrompt);
            command.Focus();
        }

        void SubmitScor4Paste(string text) {
            if(text.Trim().Equals("B",StringComparison.OrdinalIgnoreCase)){ClearMainGroup();Message("");return;}
            var destination=LayoutNumbers(text);
            if(destination.Length<1||destination.Length>4)
                throw new InvalidDataException(Scor4PastePrompt);
            Array.Resize(ref destination,4);
            var group=pendingScor4Paste;
            int staff=GroupInteger(destination[0],1,32),shift=staff-group.Staff;
            bool scale=destination[2]!=0;
            if(scale&&destination[2]<=destination[1])throw new InvalidDataException("The right position must follow the left position.");
            float factor=scale?(float)(((double)destination[2]-destination[1])/((double)group.Right-group.Left)):1;
            float origin=scale?(float)(destination[1]-(double)group.Left*factor):0;
            var appended=group.Items.Select(original=>{
                var item=original.Clone();
                GroupInteger(item.Staff+shift,1,32);
                if(shift!=0)item.Set(2,item.P(2)+shift);
                if(scale)NativeItemMotion.TransformHorizontal(item,x=>(float)(origin+(double)x*factor));
                else NativeItemMotion.Horizontal(item,destination[1]);
                if(item.Count>=4&&item.Code!=3&&item.Code!=8&&item.Code!=17&&item.Code!=18)
                    NativeItemMotion.Vertical(item,destination[3]);
                return item;
            }).ToList();
            int first=score.Items.Count+1;
            var order=score.Items.Concat(appended).ToList();
            ClearMainGroup();
            ApplyMainOrder(order,"First item # of moved group is "+first.ToString(CultureInfo.InvariantCulture).PadLeft(4));
        }
    }
}

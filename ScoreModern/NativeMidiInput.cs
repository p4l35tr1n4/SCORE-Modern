using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 public sealed partial class Editor {
  const string MidiDeviceSetting="midi-input-device";
  const string MidiHeldChordsSetting="midi-held-chords";
  static bool GroupHeldMidiPitches {get{return !NativeScoreBehaviour.Enabled&&NativeSettings.Read(MidiHeldChordsSetting)=="true";}}
  WindowsMidiInput midiInput;
  int midiGeneration;

  void AddMidiMenu(ContextMenuStrip menu){
   menu.Items.Add("MIDI input device...",null,(sender,args)=>Safe(ChooseMidiDevice));
   var chords=new ToolStripMenuItem("MIDI input: group held notes into chords");
   chords.Click+=(sender,args)=>Safe(()=>{
    NativeSettings.WriteAllText(MidiHeldChordsSetting,GroupHeldMidiPitches?"false":"true");
    if(musicInput!=null)musicInput.MidiLine=null;
   });
   menu.Items.Add(chords);
   menu.Opening+=(sender,args)=>{chords.Checked=GroupHeldMidiPitches;chords.Enabled=!NativeScoreBehaviour.Enabled;};
   Disposed+=(sender,args)=>CloseMidiInput();
  }

  void ChooseMidiDevice(){
   var devices=WindowsMidiInput.Devices();
   string selected=NativeSettings.Read(MidiDeviceSetting);
   using(var dialog=new ScoreDialog("MidiInputDevice"))
   using(var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(12),RowCount=3,ColumnCount=1})
   using(var choices=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Dock=DockStyle.Top})
   using(var accept=new Button{Text="Apply",DialogResult=DialogResult.OK,AutoSize=true,Anchor=AnchorStyles.Right}){
    dialog.Text="F2 MIDI input device";
    dialog.StartPosition=FormStartPosition.CenterParent;
    dialog.ClientSize=new Size(430,125);
    choices.Items.Add("Disabled");
    foreach(var device in devices)choices.Items.Add(device);
    choices.SelectedIndex=0;
    var current=devices.Where(device=>device.Identity==selected).ToList();
    if(current.Count==1)choices.SelectedItem=current[0];
    layout.Controls.Add(new Label{Text=devices.Count==0?"Windows reports no MIDI input devices.":"Select a device for F2 pitch and rhythm entry.",AutoSize=true});
    layout.Controls.Add(choices);
    layout.Controls.Add(accept);
    dialog.Controls.Add(layout);
    dialog.AcceptButton=accept;
    if(dialog.ShowDialog(Form.ActiveForm??this)!=DialogResult.OK)return;
    var choice=choices.SelectedItem as WindowsMidiInput.Device;
    if(choice!=null&&devices.Count(device=>device.Identity==choice.Identity)!=1)
     throw new InvalidOperationException("Windows reports multiple MIDI ports with this same identity. Use a uniquely identified MIDI port.");
    NativeSettings.WriteAllText(MidiDeviceSetting,choice==null?null:choice.Identity);
    CloseMidiInput();
    if(musicInput!=null&&musicInput.Transaction!=null)OpenMidiInput(NativePreferences.Read(source));
   }
  }

  void OpenMidiInput(ScorePreferences prefs){
   string selected=NativeSettings.Read(MidiDeviceSetting);
   if(String.IsNullOrEmpty(selected)||midiInput!=null)return;
   try{
    int baseKey=prefs.Word("MIDK",36);
    if(baseKey<0||baseKey>116)throw new InvalidOperationException("MIDK must leave room for twelve MIDI control keys.");
    var devices=WindowsMidiInput.Devices().Where(device=>device.Identity==selected).ToList();
    if(devices.Count!=1)throw new InvalidOperationException("The selected MIDI input device is unavailable or ambiguous.");
    int generation=++midiGeneration;
    midiInput=new WindowsMidiInput(devices[0],packet=>{
     if((packet&0xe0)!=0x80)return;
     try{
      BeginInvoke((Action)(()=>{
       if(generation==midiGeneration&&Enabled)Safe(()=>ReceiveMidiInput(packet,baseKey));
      }));
     }catch(InvalidOperationException){}
    });
   }catch(InvalidOperationException error){Message(error.Message);LayoutTextContents(true);}
    catch(NotSupportedException error){Message(error.Message);LayoutTextContents(true);}
  }

  void CloseMidiInput(){
   ++midiGeneration;
   if(midiInput==null)return;
   midiInput.Dispose();
   midiInput=null;
  }

  void ReceiveMidiInput(uint packet,int baseKey){
   if((packet&0xe0)!=0x80)return;
   var session=musicInput;
   if(session==null||session.Transaction==null||session.Correction>=0||session.SaveQuestion||
      session.AbandonQuestion||session.FilenameQuestion||session.SpacingPause)return;
   var stage=session.Transaction.Stage;
   if(stage!=InputStaffStage.Pitches&&stage!=InputStaffStage.Rhythms)return;
   if(session.MidiLine==null||session.MidiLine.Stage!=stage)session.MidiLine=new MidiInputLine(stage,NativeSettings.Read(MidiHeldChordsSetting)=="true");
   var line=session.MidiLine;
   int key=(int)((packet>>8)&0x7f);
   if((packet&0xf0)==0x80||(packet&0x7f0000)==0){line.Release(key);return;}
   string next;
   string current=command.Text.Substring(0,command.SelectionStart);
   try{next=line.Note(key,baseKey,current,session.Transaction.RhythmCount);}
   catch(NotSupportedException error){Message(error.Message);LayoutTextContents(true);return;}
   if(line.Notice!=null){session.MidiNotice=line.Notice;UpdateMusicInput();LayoutTextContents(true);}
   if(next==null)return;
   if(next.StartsWith(current,StringComparison.Ordinal))
    StoreInputMouseToken(next.Substring(current.Length));
   else if(stage==InputStaffStage.Rhythms&&next.Length+1==current.Length&&current.Length<command.TextLength){
    string tail=command.Text.Substring(current.Length);
    command.Text=next+(((NativeCommandBox)command).ScoreInsert?"":" ")+tail;
    command.SelectionStart=next.Length;
   }
   else{command.Text=next;command.SelectionStart=next.Length;}
   if(next.EndsWith(";",StringComparison.Ordinal)){
    string submitted=stage==InputStaffStage.Pitches?command.Text.TrimStart('/'):command.Text;
    int submittedCaret=command.SelectionStart;
    command.Clear();
    session.MidiLine=null;
    SubmitMusicInput(submitted);
    if(stage==InputStaffStage.Rhythms&&session.InputError!=null&&submittedCaret<command.TextLength)
     command.SelectionStart=submittedCaret;
   }
  }
 }
}



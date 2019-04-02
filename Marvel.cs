/*MaRBeL-1v3 MTU Anomaly Ratting Bot + salvage drones + refill drones + SAFE EWAR
take first the frigates
delete lines with new System.Media.SoundPlayer
https://forum.botengine.org/t/kaboonus-scripts-guide-wiki-page-so-do-not-respond-in-this-one/2156
**Thy Viir for helping me to optimise the code and having all that in  max filesize
*/
using BotSharp.ToScript.Extension;
using Parse = Sanderling.Parse;
using MemoryStruct = Sanderling.Interface.MemoryStruct;
using System.IO;
using System.Collections.Generic;
using System;
string VersionScript = "MARBEL-1v3 ";//do not change
//	begin of configuration section ->
 new System.Media.SoundPlayer(@"C:\sw4-force.wav").Play();
    Host.Log( "Gathering and processing some info to be used later ");
    Host.Delay(2111);
//important to change
string StationHomeName = "station1|station2";
string IgnoreNeutral = "player1|player2"; //
string MyCorpo = "[corp ticket]";
var InfoPannelRegion =
    Measurement?.InfoPanelCurrentSystem?.HeaderText.RemoveXmlTag()?.Trim();
var CurrentRegion = InfoPannelRegion.Substring(InfoPannelRegion.LastIndexOf(' ')).TrimStart();
var CurrentSystem = InfoPannelRegion.Substring(0,InfoPannelRegion.IndexOf(' ')).TrimEnd();
var MyOwnChar  = chatLocal?.ParticipantView?.Entry?.FirstOrDefault(myflag =>myflag?.FlagIcon == null);
string CharName = MyOwnChar?.NameLabel?.Text.ToLower();

string MTUName;
Dictionary<string,string> NamingMtu=new Dictionary<string,string>();
NamingMtu.Add("mychar1", "1mtu");//your name have to be in miniscule : not "MyChar" but "mychar"
NamingMtu.Add("mychar2", "2mtu");
NamingMtu.TryGetValue(CharName,out MTUName);
string WarpToAnomalyDistance = "Within 30 km";
string RattingShipName = "!Vexor|Vexor";
string SalvageShipName = "Noctis|Gnosis";
var UseMissiles = true;
string MissilesName = "Inferno Light Missile";
string QuantityMissiles = "2000";
int MinInLauncher = 19;
////usual to change
/////settings anomaly 
string AnomalyToTakeColumnHeader = "name";
string AnomalyToTake = "Forsaken Hub";
string salvagingTab = "colly|fighters";
string rattingTab = "combat|hostiles";
//
var CombatDronesFolder = "heavy";
var SalvageDronesFolder = "salvagers";
string RetreatBookmark = "home";
string messageText = "old site";

string LabelNameAttackDrones;
Dictionary<string,string> faction=new Dictionary<string,string>();
faction.Add("Delve", "Imperial Navy Praetor");
faction.Add("Fountain", "Caldari navy vespa");
faction.TryGetValue(CurrentRegion,out LabelNameAttackDrones);
 Host.Log( " Drones name : " +LabelNameAttackDrones+ "");


string UnloadDestContainerName = "Item Hangar";
string messageTextDread = " dread";
// SESSION/DT TImers
var StopOnHostiles = false;
var MaxTimeAllowed = 15;//if hostiles are in system, safety stop bot
var minutesToDT = 10;
var hoursToDT = 0;
var hoursToSession = 23;
var minutesToSession = 11;
var MinimDelayUndock = 3;
var MaximDelayUndock = 30;
/// Ranges
int?	 RangeMissilesDefault = 19999;
var shipRangerMax = 63;
int salvageRange = 5000;
int? DistanceCelestial = 30000; // or   RangeMissilesDefault
var maxDistanceToRats = 120;// you have to run from this rats
string runFromRats = "♦|Titan|Dreadnought|Autothysian";
//orbit, wrecks, etc
string celestialOrbit = "broken|pirate gate";
string CelestialToAvoid = "Chemical Factory";
string commanderNameWreck = "Commander|Dark Blood|true|Shadow Serpentis|Dread Gurista|Domination Saint|Gurista Distributor|Sentient|Overseer|Spearhead|Dread Guristas|Estamel|Vepas|Thon|Kaikka|True Sansha|Chelm|Vizan|Selynne|Brokara|Dark Blood|Draclira|Ahremen|Raysere|Tairei|Cormack|Setele|Tuvan|Brynn|Domination|Tobias|Gotan|Hakim|Mizuro";
var StopCapacitorValue = 37;
int DroneNumber = 5;
int TargetCountMax = 6;

var ActivatePerma = true;
var ActivateArmorRepairer = false;// true to be all time actived
var ActivateShieldBooster = false;
string UnsupportedArmorRepairer = "Medium 'Accommodation' Vestment Reconstructer I";
string [] PermanentActive = new [] {
"Omnidirectional", "Sensor", "Auto",



};
//////////////////
var EnterOffloadOreHoldFillPercent = 85;
var EmergencyWarpOutHitpointPercent = 45;
var StartArmorRepairerHitPoints = 95;
var StartShieldRepairerHitPoints = 55;
//
//do not change
static public Int64 Height(this RectInt rect) => rect.Side1Length();
static public bool IsScrollable(this MemoryStruct.IScroll scroll) => scroll?.ScrollHandle?.Region.Height() < scroll?.ScrollHandleBound?.Region.Height() - 4;
string LabelNameSalvageDrones = "Salvage Drone I";

//keys
var lockTargetKeyCode = VirtualKeyCode.LCONTROL;
var targetLockedKeyCode = VirtualKeyCode.SHIFT;
var orbitKeyCode = VirtualKeyCode.VK_W;
var attackDrones = VirtualKeyCode.VK_F;
var returnkey = VirtualKeyCode.RETURN;
var spacekey = VirtualKeyCode.SPACE;
var Warpkey = VirtualKeyCode.VK_S;
var Dockkey = VirtualKeyCode.VK_D;
//etc some calc
var FullCargoMessage = false;
var OldSiteExist = false;
var ImOnSite = false;
var MeChangeSalvage = false;
var SiteFinished = false;
var UseSalvageDrones = false;

DateTime maxTimeToStop;
    int Atstart = 0;
    int K=1;
    int x;
var MatchFromLabelWithRegexPattern = new Func<string, System.Text.RegularExpressions.Match>(regexPattern =>
    Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.FirstOrDefault(modulul =>modulul.TooltipLast.Value.IsWeapon ?? false)?.TooltipLast?.Value?.LabelText.Select(textus =>textus?.Text?.RegexMatchIfSuccess(regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))?.WhereNotDefault()?.FirstOrDefault());

var DistanceMinFromLabelWithRegexPattern = new Func<string, int?>(prefixPattern =>
    (int?)Distance.DistanceParseMin(MatchFromLabelWithRegexPattern(prefixPattern + Distance.DistanceRegexPattern)?.Value?.RegexMatchIfSuccess(Distance.DistanceRegexPattern)?.Value));

string ModuleSalvagerX = "Salvager";
const string StatusStringFromDroneEntryTextRegexPattern = @"\((.*)\)";
static public string StatusStringFromDroneEntryText(this string droneEntryText) => droneEntryText?.RegexMatchIfSuccess(StatusStringFromDroneEntryTextRegexPattern)?.Groups[1]?.Value?.RemoveXmlTag()?.Trim();
var startSession = DateTime.Now;
var playSession = DateTime.UtcNow.AddHours(hoursToSession).AddMinutes(minutesToSession);
var dateAndTime = DateTime.UtcNow;
var date = dateAndTime.Date;
var eveRealServerDT =date.AddHours(11).AddMinutes(-1);
if (eveRealServerDT < dateAndTime)
{
eveRealServerDT = eveRealServerDT.AddDays(1);
Host.Log(" >  eveRealServerDT :  " +eveRealServerDT.ToString(" dd/MM/yyyy HH:mm:ss")+ " .");
}
var eveSafeDT = eveRealServerDT.AddHours(-hoursToDT).AddMinutes(-minutesToDT);
Host.Log(" >  eveSafeDT : " +eveSafeDT.ToString(" dd/MM/yyyy HH:mm")+ " .");
var MaxDistanceToRats = maxDistanceToRats*1000;
var ShipRangeMax = shipRangerMax*1000;
Dictionary<string,int> dict=new Dictionary<string,int>();
dict.Add("Within 0 m",0);
dict.Add("Within 10 km",1);
dict.Add("Within 20 km",2);
dict.Add("Within 30 km",3);
dict.Add("Within 50 km",4);
dict.Add("Within 70 km",5);
dict.Add("Within 100 km",6);
dict.TryGetValue(WarpToAnomalyDistance,out x);
public string CurrentMessage;
double HocusPocusPreparatus;
double YesterdayIsk;
double Paracelsus;
double MagicalPrepare;
HocusPocusPreparatus = 0;
KaboonusTalk();
YesterdayIsk = MagicalPrepare;
Host.Log(YesterdayIsk);
HocusPocusPreparatus = YesterdayIsk;
if (!(Measurement?.IsDocked ?? false))
KaboonusTalk();
//	<- end of configuration section
Func<object> BotStopActivity = () => null;
Func<object> NextActivity = MainStep;
for(;;)
{
MemoryUpdate();
Host.Log(
		" >   " +CharName?.ToUpper()+  "    Started  in " +CurrentSystem+ " script: " +VersionScript+
		" ;   Logout in:  "  + ((TimeSpan.FromMinutes(logoutgame) < TimeSpan.Zero) ? "-" : "") + (TimeSpan.FromMinutes(logoutgame)).ToString(@"hh\:mm\:ss")+
        " ;   Sites  " + sitescount+
        " ;   Rats  : " + ListRatOverviewEntry?.Length + " ; killed " +killedratscompared+
        " ;   Local Count / Visibles : " +localCount+  " / " + MaxInLocal+ " ; "+
        " ☾⊙☽   Shield: " + ShieldHpPercent + "% ; Armor: " + ArmorHpPercent + "%" +
        "\n" +
        "                              $$$  At begining :  " +YesterdayIsk.ToString("N0" , new System.Globalization.CultureInfo("fr-FR"))+  "  $$$ Current wallet :  " +MagicalPrepare.ToString("N0" , new System.Globalization.CultureInfo("fr-FR"))+  
        "\n" +
        "                              ⊙  ISK in this session :  " + Paracelsus.ToString("N0" , new System.Globalization.CultureInfo("fr-FR"))+
        " ;  ⊙ MAx Time for hostiles   : " + MaxTimeAllowed+ "  min."+
        "\n" +
        "                              ⊙ OldSiteExist    :    " +OldSiteExist+
        " ;  ⊙ Use Drones for Salvage :  " +UseSalvageDrones+

        " ;  ⊙ Site Finished   : " +SiteFinished+
        "\n" +
		"                               >>  Hostiles: " +(chatLocal?.ParticipantView?.Entry?.Count(IsNeutralOrEnemy)-1)+ " ignored: "  +IgnoreListCount + "  Drones: (All/heavy): "  +(DronesInSpaceCount + DronesInBayCount)+ "( "+ (DronesInBayCombatFolderCount + DronesInSpaceCombatFolderCount) +  " )"+" space:"+ DronesInSpaceCount +  

		" ;   Targets:  " + Measurement?.Target?.Length+
		" ;   Cargo: " + OreHoldFillPercent + "%" +
		" ;   Salvagers (inactive): " + SetModuleSalvager?.Length + "(" + SetModuleSalvagerInactive?.Length +
		" ;   Wrecks: " + ListWreckOverviewEntry?.Length+
        "\n" +
        "                                                 <<<<<☾⊙☽>> Retreat Msg : "  + RetreatReason +
        "\n" +
        "                              <<☾⊙☽>> Last or Current  Msg:  " + CurrentMessage+ "" +
        "\n" +

        "\n" +
        "                              <<☾⊙☽>>>>>>>> My Ship EWAR  II :    "  + MyShipEwarCombo+
        "\n" +
        "                             <<☾⊙☽>>          Ship Indication:  " + FirstIndication+ "" +
        "\n" +
        "                             <<☾⊙☽>>          NextAct:  <<☾⊙☽>>  " + NextActivity?.Method?.Name);
CloseModalUIElement();

if(Measurement?.WindowTelecom != null)
    CloseWindowTelecom();
while (SpeedWarping)
{
    while(K>0)
    {
        CheckLocation();
        K--;
    }

    if ( KmIndication)
    {
        var probeScannerWindow = Measurement?.WindowProbeScanner?.FirstOrDefault();
        var scanActuallyAnomaly = probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(ActuallyAnomaly);
        if ((null != scanActuallyAnomaly) && 0 < listOverviewEntryFriends?.Length && ListCelestialObjects?.Length > 0)
            ClickMenuEntryOnMenuRoot(scanActuallyAnomaly, "Ignore Result");
        if (OldSiteExist && 0 < listOverviewEntryFriends?.Length && ListCelestialObjects?.Length > 0)
        deleteBookmark();
    }

}
if(Measurement?.WindowOther != null)
    CloseWindowOther();

if (Tethering)
{
    ModuleStopToggle(ModuleAfterburner);
    ModuleStopToggle(ModuleArmorRepairer?.FirstOrDefault());
}
if (Measurement?.IsDocked ?? false)
    MainStep();
if(null != RetreatReason && !(Measurement?.IsDocked ?? false))
{

    if (!Tethering)
    {
        ModuleStopToggle(ModuleAfterburner);
        DroneEnsureInBay();
        ActivateArmorExecute();
        if (Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(m => m?.TooltipLast?.Value?.IsShieldBooster ?? false) >0)
        	ModuleToggle(ModuleShieldBooster);
        if (listOverviewEntryEnemy?.Length > 0)
        {
            WarpInstant ();
        }
        if (!Aligning && ReadyForManeuver && !Tethering )
        {
            Sanderling.KeyDown(VirtualKeyCode.VK_A);
            Sanderling.MouseClickLeft(listOverviewStationName?.FirstOrDefault());
            Sanderling.KeyUp(VirtualKeyCode.VK_A);
            Messages("               align");
        }
        DroneEnsureInBay();

        if (null !=RetreatReasonDread)
        {
        var probeScannerWindow = Measurement?.WindowProbeScanner?.FirstOrDefault();
            var scanActuallyAnomaly = probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(ActuallyAnomaly);
            Host.Log("               Runing from dread");
            deleteBookmark();
            SavingLocation ();
        if (null != scanActuallyAnomaly)
            {
                ClickMenuEntryOnMenuRoot(scanActuallyAnomaly, "Ignore Result");
            }
        }
        if(0 == DronesInSpaceCount)
        {
            if (listOverviewStationName?.Length > 0 &&( null != RetreatReason) && !SpeedWarping)
                WarpInstant ();
            if (ReadyForManeuver  &&  null == WindowDrones )
                {
                    Messages("              warping home");
                    WarpingSlow(RetreatBookmark, "dock");
                    Console.Beep(1500, 200);
                    K=1;
                }
        }
        else
            DroneEnsureInBay();
    }
    if (null != RetreatReason  &&  Tethering &&  ReadyForManeuver)
    {
        Host.Log("               retreat On Tethering Zone");
        MainStep ();
    }
    continue;
}
NextActivity = NextActivity?.Invoke() as Func<object>;
if(BotStopActivity == NextActivity)
	break;
if(null == NextActivity)
	NextActivity = MainStep;
Host.Delay(1111);
}
//
 int? MaxInLocal => (int)(chatLocal?.ParticipantView?.Region.Height() / 18.7);
int? localCount => chatLocal?.ParticipantView?.Entry?.Count();
int? HulHpPercent => ShipUi?.HitpointsAndEnergy?.Struct / 10;
int? ShieldHpPercent => ShipUi?.HitpointsAndEnergy?.Shield / 10;
int? ArmorHpPercent =>(!(Measurement?.IsDocked ?? false) && !(Measurement?.IsUnDocking ?? false)) ? ShipUi?.HitpointsAndEnergy?.Armor / 10 : 100;
int? CapacitorHpPercent => ShipUi?.HitpointsAndEnergy?.Capacitor / 10;
int? LastCheckNumberRatsSynced = null;
int? LastCheckOreHoldFillPercent = null;
int sitescount = 0;
int killedratscompared = 0;
string MyShipEwarCombo => String.Join(" ; ", Measurement?.ShipUi?.EWarElement?.Where(m=> !string.IsNullOrEmpty(m.EWarType))?.Select(m =>m?.EWarType) ?? new string[0]);
string FirstIndication => Measurement?.ShipUi?.Indication?.LabelText?.FirstOrDefault().Text.EmptyIfNull().ToString();
public bool KmIndication => Measurement?.ShipUi?.Indication?.LabelText?.Any(indicationLabel => (indicationLabel?.Text).RegexMatchSuccessIgnoreCase("km")) ?? false;
public bool tooManyOnLocal => (chatLocal?.ParticipantView?.Scroll?.IsScrollable() ?? true)|| (MaxInLocal < (localCount+2));
bool activeshipnameSalvage => WindowInventory?.ActiveShipEntry?.Text?.RegexMatchSuccessIgnoreCase(SalvageShipName) ?? false;
bool activeshipnameRatting => WindowInventory?.ActiveShipEntry?.Text?.RegexMatchSuccessIgnoreCase(RattingShipName) ?? false;
bool DefenseExit =>
    (Measurement?.IsDocked ?? false) ||
    !(0 < ListRatOverviewEntry?.Length
    || ListRatOverviewEntry?.FirstOrDefault()?.DistanceMax > MaxDistanceToRats);
bool DefenseEnter => !DefenseExit;
string RetreatReasonHostiles = null;
string RetreatReasonArmor = null;
string RetreatReasonBumped = null;
string RetreatReasonCapsuled = null;
string RetreatReasonTimeElapsed = null;
string RetreatReasonDrones = null;
string RetreatReasonCargoFull = null;
string SiteFinishRetreat = null;
string ChangingToSalvager = null;
string RetreatReasonDread = null;
string RetreatReason => RetreatReasonArmor ?? RetreatReasonBumped
        ?? RetreatReasonCapsuled ?? RetreatReasonTimeElapsed
        ?? RetreatReasonHostiles ?? RetreatReasonDrones
        ?? RetreatReasonCargoFull ?? SiteFinishRetreat
        ?? ChangingToSalvager ?? RetreatReasonDread ?? null;

bool OreHoldFilledForOffload => Math.Max(0, Math.Min(100, EnterOffloadOreHoldFillPercent)) <= OreHoldFillPercent;
//

Func<object> MainStep()
{
    while (ReadyForManeuverNot)
    {
        Host.Delay(2111);
        Host.Log("               docking/warping ");
    return InBeltMineStep;
    }
    EnsureWindowInventoryOpen();
    EnsureWindowInventoryOpenActiveShip();
if (Measurement?.IsDocked ?? false)
    {

        while ( K>0)
            {
                var enteringStationTime = DateTime.Now;
                maxTimeToStop = enteringStationTime.AddMinutes(MaxTimeAllowed);
            KaboonusTalk();
            ReviewSettings();
                K--;
            return MainStep;
            }
        if ( ReasonTimeElapsed || ReasonCapsuled ||ReasonDrones || ( !activeshipnameRatting && !activeshipnameSalvage))
        {
        Host.Log("               bot stop: Capsule:  " +ReasonCapsuled+ "  Drones : " +ReasonDrones+ " time :  " +ReasonTimeElapsed+ "" );
        if ( ReasonTimeElapsed )
            Sanderling.KillEveProcess();
        Host.Delay(3111);
            return BotStopActivity;
        }
        while (YesHostiles || tooManyOnLocal)
            {
                var checkingTimeToStop = DateTime.Now;
                    Host.Delay(4111);
                Messages("               hostiles time allowed:  now  :  " + checkingTimeToStop+ "  ?? max  " +maxTimeToStop+ "");
                    Host.Delay(4111);
            if(StopOnHostiles &&(checkingTimeToStop > maxTimeToStop))
                return BotStopActivity;
            else
                return MainStep;
            }
        CheckLocation();
        if ( activeshipnameRatting || activeshipnameSalvage)
            {
            Host.Log("               activeshipname :  " +WindowInventory?.ActiveShipEntry?.Text.ToString());
            InInventoryUnloadItems();
            }
        if (ChangingToSalvager != null)
            ChangeToSalvage ();
        if (SiteFinished == true ||  SiteFinishRetreat != null)
            ChangeToRatting () ;
        Sanderling.WaitForMeasurement();
        Host.Delay(4111);
         EnsureWindowInventoryOpen();

        if (activeshipnameRatting )
            {
            NoQuantity = true;
            Refill ();
            }

        RepairShop ();
        UseSalvageDrones = false;
        SiteFinished = false;
        FullCargoMessage = false;
        Host.Delay(4111);
        if (!YesHostiles || !tooManyOnLocal)
            {
            Random rnd = new Random();
            int DelayTime = rnd.Next(MinimDelayUndock, MaximDelayUndock);
            Host.Log("               Keep your horses for :  " + DelayTime+ " s ");
            Host.Delay( DelayTime*1000);
            while (Measurement?.IsDocked ?? false || (!Tethering  && MySpeed>100))
                {
                    if (Measurement?.WindowStation?.FirstOrDefault()?.UndockButton != null)
                        Sanderling.MouseClickLeft(Measurement?.WindowStation?.FirstOrDefault()?.UndockButton);
                    Host.Log("             Feel the Force Luke!");
                    Host.Delay(1823);
                }
            if  (!(Measurement?.IsDocked ?? false) || (!Tethering  && MySpeed>100))
                {
                Host.Log("               Luke > I try Master Yoda, ... I try!");
                Host.Delay(5823);
                Sanderling.KeyboardPressCombined(new[]{ lockTargetKeyCode, spacekey});
                Sanderling.WaitForMeasurement();
                CheckLocation();
                return NearStation;
                }
            }
            else
            {
            var enteringStationTime = DateTime.Now;
            maxTimeToStop = enteringStationTime.AddMinutes(MaxTimeAllowed);
            return MainStep;
            }
    }
if (ReadyForManeuver)
    {

        if(!OldSiteExist )
            CheckLocation();
        EnsureWindowInventoryOpen();
        EnsureWindowInventoryOpenActiveShip();
        if (0 < DronesInSpaceCount  &&  NoRatsOnGrid)
            DroneEnsureInBay();
        if(null != RetreatReason && !ReadyForManeuverNot  && listOverviewStationName?.FirstOrDefault()?.DistanceMin < 2000000)
            {
                	   while (DronesInSpaceCount>0 )
	        DroneEnsureInBay();
            Host.Log("               retreat  main step !   " + RetreatReason);
            WarpingSlow(RetreatBookmark, "dock");
            return MainStep;
            }
        if (Tethering)
        {
            if (DronesInBayFromFoldersCount < DronesInBayCount  && activeshipnameRatting )
                MoveDronesToFolder();
            if ( OreHoldFillPercent > 33)
                {
                Host.Log("                cargo at : " +OreHoldFillPercent+ " %  . Go to unload !");
                WarpingSlow(RetreatBookmark, "dock");
                return MainStep;
                }
            if (HulHpPercent < 100 ||ArmorHpPercent < 100  ||ShieldHpPercent < 100)
               while (HulHpPercent < 100 ||ArmorHpPercent < 50)
                {
                Host.Log("               Luke > I try Master Yoda, ... I try ... to refill my HP !");
                Host.Delay(5823);
                }
        }

        if (!OldSiteExist && activeshipnameSalvage )
            {
            SiteFinished = true;
            Host.Log("               Salvage ship but no   old site bookmark => go home  !");
            WarpingSlow(RetreatBookmark, "dock");
            }
        if ( !activeshipnameRatting && !activeshipnameSalvage)
            {
            Host.Log("               Unregistered ship name, go home  !");
            WarpingSlow(RetreatBookmark, "dock");
            }
        if(!Dockinginstance || !( Measurement?.IsDocked ?? false))
            ModuleMeasureAllTooltip();




        if (ActivatePerma  )
            PermaExecute();

        if (0 == DronesInSpaceCount  )
        {
            Host.Log("               ready for rats ");
            if( OldSiteExist && (!YesHostiles || !tooManyOnLocal)) //&& Tethering 
                ReturnToOldSite ();
            if ( !Tethering &&  ( ReadyToBattle  || LookingAtStars || ImOnSite))
                return InBeltMineStep;
            if (!OldSiteExist && activeshipnameRatting && listOverviewMtu?.Length  == 0  && (!YesHostiles || !tooManyOnLocal))
                return TakeAnomaly;
        }
    }
return InBeltMineStep;
}
//

Func<object>  NearStation()
{
    if (BackOnMainStep)
        return MainStep;
    Host.Delay(8044);
    var probeScannerWindow = Measurement?.WindowProbeScanner?.FirstOrDefault();
    if (!( Measurement?.IsDocked ?? false))
    {
        Sanderling.WaitForMeasurement();
        while (!Tethering  && MySpeed>100)
        {
        Sanderling.KeyboardPressCombined(new[]{ lockTargetKeyCode, spacekey});
            Host.Delay(3444);
        }
        EnsureWindowInventoryOpen();
        if (probeScannerWindow == null)
            FlashWindowProbes();
        if ( DronesInBayListFolderSalvageEntry != null)
            UseSalvageDrones = true;
        else
            UseSalvageDrones = false;
        if(!Dockinginstance || !( Measurement?.IsDocked ?? false))
            ModuleMeasureAllTooltip();
        Collapse(SalvageDronesFolder);
        if (UseMissiles && !(ModuleWeapon?.IsNullOrEmpty() ?? false) && (ModuleWeapon?.FirstOrDefault().TooltipLast.Value.LabelText.ElementAtOrDefault(1).Text.RegexMatchSuccessIgnoreCase("launcher") ?? false)
		&& ( (!ModuleWeapon?.FirstOrDefault()?.TooltipLast?.Value?.LabelText?.ElementAtOrDefault(2)?.Text?.IsNullOrEmpty() ?? false)
		? ((int?)ModuleWeapon?.FirstOrDefault().TooltipLast.Value.LabelText.ElementAtOrDefault(2).Text?.Substring(0 , 2).TryParseInt()) : 0 )< MinInLauncher )
        ClickMenuEntryOnMenuRoot(ModuleWeapon?.FirstOrDefault(),"reload all");

    }
return MainStep;
}
//


Func<object> TakeAnomaly()
{
        Host.Log("               take Anomaly func");
        SiteFinished = false;
    ModuleMeasureAllTooltip();
    if (DronesInBayFromFoldersCount < DronesInBayCount )
    {
        new System.Media.SoundPlayer(@"C:\Appear.wav").Play();
    if (DronesInBayFromFoldersCount < DronesInBayCount  && activeshipnameRatting )
        MoveDronesToFolder();
    }
    if (combatTab != OverviewTabActive )
    {
        Sanderling.MouseClickLeft(combatTab);
        Host.Delay(311);
    }
    DroneEnsureInBay();
    Collapse(SalvageDronesFolder);
var AmmoInventoryCount =(WindowInventory?.SelectedRightInventory?.ListView?.Entry?.FirstOrDefault( item =>item?.ListColumnCellLabel.FirstOrDefault().Value.RegexMatchSuccessIgnoreCase(MissilesName) ?? false) != null) ? Regex.Replace(WindowInventory?.SelectedRightInventory?.ListView?.Entry?.FirstOrDefault( item =>item?.ListColumnCellLabel.FirstOrDefault().Value.RegexMatchSuccessIgnoreCase(MissilesName) ?? false)
.ListColumnCellLabel.ElementAtOrDefault(1).Value, "[^0-9]+", "").TryParseInt() : 0;

	if ( OreHoldFillPercent > 29 || activeshipnameSalvage || (UseMissiles && AmmoInventoryCount < 500 ))
    {
        Host.Log("               Cargo at : " +OreHoldFillPercent+ " %  ;  Ammo Inventory Count : " +AmmoInventoryCount+"   Go to unload !");
        WarpingSlow(RetreatBookmark, "dock");
        return MainStep;
    }
    if (OldSiteExist && activeshipnameRatting && Tethering)
    {
        ReturnToOldSite ();
        return MainStep;
    }
    var probeScannerWindow = Measurement?.WindowProbeScanner?.FirstOrDefault();
    var scanActuallyAnomaly = probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(ActuallyAnomaly);
    var UndesiredAnomaly = probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(IgnoreAnomaly);
    var scanResultCombatSite = probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(AnomalySuitableGeneral);
    if ( (DronesInSpaceCount + DronesInBayCount ) < DroneNumber)
	{
        Sanderling.InvalidateMeasurement();
            Sanderling.WaitForMeasurement();
        if ( (DronesInSpaceCount + DronesInBayCount ) < DroneNumber)
	        reasonDrones = true;
	}
    if (probeScannerWindow == null)
        FlashWindowProbes();
    if (null != scanActuallyAnomaly)
    {
        ClickMenuEntryOnMenuRoot(scanActuallyAnomaly, "Ignore Result");
        return TakeAnomaly;
    }
    if (null != UndesiredAnomaly)
    {
        ClickMenuEntryOnMenuRoot(UndesiredAnomaly, "Ignore Result");
        Host.Log("               working at ignoring anomalies :) be patient");
        return TakeAnomaly;
    }
    if ((null != scanResultCombatSite) && (null == UndesiredAnomaly))
    {
        Sanderling.MouseClickRight(scanResultCombatSite);
        Sanderling.WaitForMeasurement();
        var menuResult = Measurement?.Menu?.ToList();
        if (null == menuResult)
        {
            Host.Log("                R2D2 fails: not expected  menu!  ");
            return TakeAnomaly;
        }
		else
		{
            var menuResultWarp = menuResult?[0].Entry.ToArray();
            var menuResultSelectWarpMenu = menuResultWarp?[1];
        Sanderling.MouseClickLeft(menuResultSelectWarpMenu);
            Sanderling.WaitForMeasurement();
            var menuResultats = Measurement?.Menu?.ToList();
		if (Measurement?.Menu?.ToList() ? [1].Entry.ToArray()[x].Text !=  WarpToAnomalyDistance)
			{
			    return TakeAnomaly;
			}
			else
			{
			var menuResultWarpDestination = Measurement?.Menu?.ToList() ? [1].Entry.ToArray();
			Messages("               The Force be with you, in to the next journey to : " +AnomalyToTake+ "  . ");
			ClickMenuEntryOnMenuRoot(menuResultWarpDestination[x], WarpToAnomalyDistance);
            Host.Log("    ChangingToSalvager ? :    " +ChangingToSalvager+ ""); 
			if (probeScannerWindow != null)
			FlashWindowProbes();
            return InBeltMineStep;
			}
		}
    }
    if (null == scanResultCombatSite && Tethering && MySpeed >2)
        Sanderling.KeyboardPressCombined(new[]{ lockTargetKeyCode, spacekey});
                   if (null == scanResultCombatSite && Tethering && MySpeed < 2)
            {
                Host.Log("               R2D2: No anomalies, waiting ");
                return MainStep;
            }
    if (null == scanResultCombatSite && !Tethering)
        {
            while ( L>0)
            {
                if (null == scanResultCombatSite && !Tethering)
                {
                Host.Log("               Trust the Force, Luke  " +L+ "  . ");
                L--;
                return MainStep;
                }
            }
            if (null == scanResultCombatSite && !Tethering)
            {
                Host.Log("               R2D2: no more anomalies! ");
                WarpingSlow(RetreatBookmark, "warp|approach");
            }
        }
    return MainStep;
}
//

Func<object> DefenseStep()
{
    if (BackOnMainStep)
        return MainStep;
    if (ActivatePerma  )
        PermaExecute();
    if (combatTab != OverviewTabActive )
    {
    Sanderling.MouseClickLeft(combatTab);
    Host.Delay(311);
    }
    Host.Log("               Defense step " );
        var shouldAttackTarget = (ListRatOverviewEntry?.Any(entry => entry?.MainIconIsRed ?? false) ?? false) && (null ==Measurement?.Target?.FirstOrDefault()?.LabelText?.FirstOrDefault(label => label?.Text?.RegexMatchSuccess(MTUName, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false));
    var droneInLocalSpaceIdle =
        AllDrones?.Any(drone =>drone?.LabelText?.FirstOrDefault()?.Text?.RegexMatchSuccessIgnoreCase("Idle") ?? false ) ?? false;
    var droneInLocalSpaceFight =
        AllDrones?.Any(drone =>drone?.LabelText?.FirstOrDefault()?.Text?.RegexMatchSuccessIgnoreCase("Fighting") ?? false ) ?? false;

    CountingRatsUpdate();
    if(null != RetreatReason && !Tethering && !(Measurement?.IsDocked ?? false))
    {
                ActivateArmorExecute();
    }
    if (Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(m => (m?.TooltipLast?.Value?.IsArmorRepairer ?? false) || (m?.TooltipLast?.Value?.LabelText?.Any(
                label => label?.Text?.RegexMatchSuccess(UnsupportedArmorRepairer, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false) ?? false))>0)
    {
        if (ActivateArmorRepairer == true || ArmorHpPercent < StartArmorRepairerHitPoints)
        {
            Host.Log("               Armor integrity < "  + StartArmorRepairerHitPoints + "%");
            ActivateArmorExecute();
        }
        if (ArmorHpPercent > StartArmorRepairerHitPoints && ActivateArmorRepairer == false )
            ModuleStopToggle(ModuleArmorRepairer?.FirstOrDefault());
    }
    if (Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(m => m?.TooltipLast?.Value?.IsShieldBooster ?? false) >0)
    {
        if (ActivateShieldBooster == true || ShieldHpPercent < StartShieldRepairerHitPoints)
        {
            Host.Log("               Shield integrity < "  + StartShieldRepairerHitPoints + "%");
            ModuleToggle(ModuleShieldBooster);
        }
        if (ShieldHpPercent > StartShieldRepairerHitPoints && ActivateShieldBooster == false )
            ModuleStopToggle(ModuleShieldBooster);
    }
    if (Measurement?.ShipUi?.Indication?.ManeuverType != ShipManeuverTypeEnum.Orbit)
        {
            Orbitkeyboard();
        }

	Expander(CombatDronesFolder);
    
    if (!ListRatOverviewEntry?.Where( x => !x.EWarType?.IsNullOrEmpty() ?? false)?.IsNullOrEmpty() ?? false && (EWarToAttack?.FirstOrDefault()?.MeTargeted ?? false))
    {
    Sanderling.MouseClickLeft(Measurement?.Target?.FirstOrDefault( target =>((target?.DistanceMax.Value == EWarToAttack?.FirstOrDefault()?.DistanceMax.Value )&&( target ?.TextRow.Contains( EWarToAttack?.FirstOrDefault()?.Name) ?? false))));  
    Sanderling.KeyboardPress(attackDrones);
    Host.Log(" ewared");
    Console.Beep(1500, 200);
    }
   
    var NPCtargheted = !Measurement?.Target?.IsNullOrEmpty() ?? false ? Measurement?.Target?.Length : 0;
    if(FrigateListRatOverviewEntry.Length > 0 && !(FrigateListRatOverviewEntry.Any(frigates => frigates?.MeTargeted ?? false)) &&  NPCtargheted < TargetCountMax ) 
        {
        for (int i = 0; FrigateListRatOverviewEntry.Length<7 ? i <FrigateListRatOverviewEntry.Count() : i<7 ; i++)
        {
        Sanderling.KeyDown(lockTargetKeyCode);
        Sanderling.MouseClickLeft(FrigateListRatOverviewEntry.ElementAtOrDefault(i));
        Sanderling.KeyUp(lockTargetKeyCode);
        continue;
        }
        }
    if (!(FrigateListRatOverviewEntry.Length > 0) && !(ListRatOverviewEntry.Any(npc =>npc?.MeTargeted ?? false)) && NPCtargheted < TargetCountMax)// && 1 < ListRatOverviewEntry?.Length)
    {
        for (int i = 0; ListRatOverviewEntry.Length<TargetCountMax ? i <ListRatOverviewEntry.Length : i<TargetCountMax ; i++)
        {
            Sanderling.KeyDown(lockTargetKeyCode);
        Sanderling.MouseClickLeft(ListRatOverviewEntry.ElementAtOrDefault(i));
            Sanderling.KeyUp(lockTargetKeyCode);  
        continue;
        }
    }
    if (!Measurement?.Target?.IsNullOrEmpty() ?? false)
    {
        var TargetList = Measurement?.Target
            ?.OrderBy(entry => entry?.TextRow?.ToString()?.RegexMatchSuccessIgnoreCase(@"Domination|Commander|Dread|True|Shadow|Sentient"))
            ?.OrderBy(entry => entry?.TextRow?.ToString()?.RegexMatchSuccessIgnoreCase(@"battery|tower|sentry|web|strain|splinter|render|raider|friar|reaver|guardian|loyal|dire|elder|arch"))
            ?.OrderBy(entry => entry?.TextRow?.ToString()?.RegexMatchSuccessIgnoreCase(@"coreli|centi|alvi|pithi|corpii|gistii|cleric|engraver")) //Frigate
            ?.OrderBy(entry => entry?.TextRow?.ToString()?.RegexMatchSuccessIgnoreCase(@"corelior|centior|alvior|pithior|corpior|gistior")) //Destroyer
            ?.OrderBy(entry => entry?.TextRow?.ToString()?.RegexMatchSuccessIgnoreCase(@"corelum|centum|alvum|pithum|corpum|gistum|prophet")) //Cruiser
            ?.OrderBy(entry => entry?.TextRow?.ToString()?.RegexMatchSuccessIgnoreCase(@"corelatis|centatis|alvatis|pithatis|corpatis|gistatis|apostle")) //Battlecruiser
            ?.OrderBy(entry => entry?.TextRow?.ToString()?.RegexMatchSuccessIgnoreCase(@"core\s|centus|alvus|pith\s|corpus|gist\s")) //Battleship
            //?.ThenBy(entry => entry?.DistanceMax?? int.MaxValue)
            ?.ToArray();

        if (shouldAttackTarget)
        {
        if (ModuleWeapon?.Length>0  && Measurement?.Target?.FirstOrDefault(target => target?.IsSelected ?? false).DistanceMax < WeaponMaxRange)
            { 
            Sanderling.MouseClickLeft(TargetList.FirstOrDefault());
            ShootWeapon();
            }


		if (droneInLocalSpaceIdle && (Measurement?.Target?.Length > 0))
			{
                Sanderling.MouseClickLeft(TargetList.FirstOrDefault());
                ActivatePainterExecute();
				Sanderling.KeyboardPress(attackDrones);
				Messages("               Vipers engage the target " );
            }
	    }
        else
            UnlockTarget();
    }

    if (droneInLocalSpaceIdle && ListRatOverviewEntry?.FirstOrDefault()?.DistanceMax > ShipRangeMax)
        OrbitRats();

   if (EWarToAttack?.Length > 0)
    {

        var EWarTargeted = EWarToAttack?.FirstOrDefault(target => target?.MeTargeted ?? false);
        if (EWarTargeted == null)
        {
            Sanderling.KeyDown(lockTargetKeyCode);
            Sanderling.MouseClickLeft(EWarToAttack?.FirstOrDefault(entry => !((entry?.MeTargeted ?? false))));
            Sanderling.KeyUp(lockTargetKeyCode);
        }
        else 
            Sanderling.MouseClickLeft(Measurement?.Target?.FirstOrDefault( target =>((target?.DistanceMax.Value == EWarToAttack?.FirstOrDefault()?.DistanceMax.Value )&&( target ?.TextRow.Contains( EWarToAttack?.FirstOrDefault()?.Name) ?? false))));  
        new System.Media.SoundPlayer(@"C:\attack.wav").Play();
        Sanderling.KeyboardPress(attackDrones);
        Host.Log("               '"  + EWarTargeted?.Name+  "' .  This  " +EWarTargeted?.RightIcon?.FirstOrDefault()?.HintText+ " ...!");
    }

    if ( (DronesInSpaceCount + DronesInBayCount ) < DroneNumber)
    {
        Sanderling.InvalidateMeasurement();
        Sanderling.WaitForMeasurement();
        if ( (DronesInSpaceCount + DronesInBayCount ) < DroneNumber )
        reasonDrones = true;
    }


    if (0 < DronesInBayCount && DronesInSpaceCount < DroneNumber)
        DroneLaunch();
    if (DefenseExit)
    {
            Host.Delay(4311);// lastly a new wave appear so we check in 4sec
        Sanderling.WaitForMeasurement();
        if (DefenseEnter)
            return DefenseStep;
        ModuleStopToggle(ModuleAfterburner);
        while (DronesInSpaceCount>0 )
                DroneEnsureInBay();
        new System.Media.SoundPlayer(@"C:\Appear.wav").Play();
        if (MySpeed>100)
            Sanderling.KeyboardPressCombined(new[]{ lockTargetKeyCode, spacekey});
        KaboonusTalk();
        Messages(" defense ended");
        if (listOverviewMtu?.Length >0)
        {
            Host.Log(" MTU  COUNT " +listOverviewMtu?.Length );
            MeChangeSalvage = true;
            //return InBeltMineStep;
        }
        else
        {
        MeChangeSalvage = false;
        return InBeltMineStep;
        }
    }
        return DefenseStep;
}
//


Func<object> InBeltMineStep()
{

 	if(OreHoldFilledForOffload || FullCargoMessage)
		return null;
    if (BackOnMainStep)
        {
        Host.Log("               tethering zone, not on site!");
        return MainStep;
        }

    if (!ReadyForManeuver || SpeedWarping)
        return InBeltMineStep;
    if (ActivatePerma )
        PermaExecute();



    var InventoryListItem = WindowInventory?.SelectedRightInventory?.ListView?.Entry?.ToArray();
    var MtuItem = WindowInventory?.SelectedRightInventory?.ListView?.Entry?.FirstOrDefault()?.LabelText?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(MTUName) ?? false);
    var probeScannerWindow = Measurement?.WindowProbeScanner?.FirstOrDefault();

    if (probeScannerWindow == null)
        FlashWindowProbes();
    EnsureWindowInventoryOpen();
    EnsureWindowInventoryOpenActiveShip();

    if (activeshipnameRatting && !SpeedWarping)
    {
        if (combatTab != OverviewTabActive)
        {
            Sanderling.MouseClickLeft(combatTab);
                Host.Delay(1111);
        }
        Messages("ratting mode? :   " +WindowInventory?.ActiveShipEntry?.Text+  "  " );
    if (BackOnMainStep)
        {
        Host.Log("               tethering zone, not on site!");
        return null;
        }
        if ( (0 < listOverviewEntryFriends?.Length || ListCelestialToAvoid?.Length > 0 ) 
        && ReadyToBattle)
        {
            if (  ListCelestialToAvoid?.Length > 0)
            {
                    Host.Log("               Gas Haven, better run!!");
                WarpInstant();
            }
            if (Measurement?.ShipUi?.Indication?.ManeuverType != ShipManeuverTypeEnum.Orbit )
            {
                    Messages("               Friends on site!");
                ActivateArmorExecute();
                if (listOverviewMtu?.Length >0 )
                    RecoverMtu ();
                if (OldSiteExist)
                    deleteBookmark ();
                return TakeAnomaly;
            }
        }

        if (ReadyToBattle && (Measurement?.ShipUi?.Indication?.ManeuverType != ShipManeuverTypeEnum.Orbit))
        {
                Host.Log("               Prepare to Battle");
            if (!OldSiteExist)
                SavingLocation ();
            if (listOverviewMtu?.Length  == 0 && MtuItem != null  )
                LaunchMtuforSelf ();
            Orbitkeyboard();
            if (DefenseEnter)
                return DefenseStep;
        }
        if (DefenseEnter)
            return DefenseStep;

        if (  LookingAtStars && 0 <= ListCelestialObjects?.Length && !Tethering && 0 < ListWreckOverviewEntry.Length)
        {
            while (0 < listOverviewCommanderWreck.Length )
            {
                var LootButton = Measurement?.WindowInventory?[0]?.ButtonText?.FirstOrDefault(text => text.Text.RegexMatchSuccessIgnoreCase("Loot All"));
                if (LootButton != null)
                    Sanderling.MouseClickLeft(LootButton);
                if ( listOverviewCommanderWreck?.FirstOrDefault()?.DistanceMax > 1200)
                    ClickMenuEntryOnMenuRoot(listOverviewCommanderWreck?.FirstOrDefault(), "open cargo");
            }

            DroneEnsureInBay();
            if (listOverviewMtu?.Length >0)
            {
                MeChangeSalvage = true;
                return MainStep;
            }
            else
            {
                if ( DronesInBayListFolderSalvageEntry != null )
                    UseSalvageDrones = true;
                else
                    UseSalvageDrones = false;
                if (UseSalvageDrones)
                {
                    if( 0 < ListWreckOverviewEntry.Length)
                        return DronesAreSalvagingStep;
                    else
                        return   FinishingAsite;
                }
                if (!UseSalvageDrones  )
                {
                    Messages("               Try to take another anomaly");
                    return   FinishingAsite;
                }
            }
        }
        if (ListWreckOverviewEntry.Length == 0 && ListCelestialObjects?.Length >= 0 && !Tethering && 0 == ListRatOverviewEntry?.Length )
        {
            if (listOverviewMtu?.Length >0 )
            {
                MeChangeSalvage = true;
            }
            else
                return FinishingAsite;
        }
        return InBeltMineStep;
    }


    if (activeshipnameSalvage)
    {
        Messages("active ship is  salvager ? :  " +WindowInventory?.SelectedRightInventoryPathLabel?.Text+ "  " );
        if (salvageTab != OverviewTabActive)
            Sanderling.MouseClickLeft(salvageTab);

        if (Tethering)
        {
                Host.Log("               tethering zone, irregular situation, dock!");
            CheckLocation();
            if (OldSiteExist)
                ReturnToOldSite ();
            else
                WarpingSlow(RetreatBookmark, "dock");
        }
        
        if (DefenseEnter)
        {
            Messages("               Salvage mode,  running from rats");
            WarpInstant ();
        }
        EnsureWindowInventoryOpen();
        if (!OreHoldFilledForOffload && !Tethering && LookingAtStars  && ListWreckOverviewEntry.Length >=0)
            {
            Host.Log("               general");
            if ( ListWreckOverviewEntry?.FirstOrDefault()?.DistanceMax > salvageRange  && ReadyForManeuver)
            {
                Messages("               waiting the wrecks");
                return SalvagingStep;
            }
            if (ListWreckOverviewEntry.Length == 0 && ReadyForManeuver)
            {
   
                if (!Tethering && LookingAtStars && ListWreckOverviewEntry.Length ==0)
                {
                    Messages("                Im coolest! Site finished! Recover the mtu and ... Go Home!! ");
                    return FinishingAsite;
                }
            }
            else
                return SalvagingStep;
            }
    }
return InBeltMineStep;
}
///////////////////////////////

public bool AnyDroneIdle =>AllDrones?.Any(drone =>drone?.LabelText?.FirstOrDefault()?.Text?.Contains("Idle") ?? false ) ?? false;

Func<object> DronesAreSalvagingStep()

{
    Host.Log("               Drones Are Salvaging step");
    if (BackOnMainStep )
        return null;


    if (salvageTab != OverviewTabActive)
    {
        Sanderling.MouseClickLeft(salvageTab);
            Host.Delay(1111);
    }

    Expander(SalvageDronesFolder);

    if (UseSalvageDrones && 0 < ListWreckOverviewEntry.Length && DronesInBayListFolderSalvageEntry != null)
    {
        if ( DronesInBayListFolderSalvageEntry != null && DronesInSpaceCount == 0 )
        {
            ClickMenuEntryOnMenuRoot(Measurement?.ShipUi?.Center, "Reconnect to lost drones");
                Sanderling.MouseClickRight(DronesInBayListFolderSalvageEntry);
            Sanderling.MouseClickLeft(Menu?.FirstOrDefault()?.EntryFirstMatchingRegexPattern("launch", RegexOptions.IgnoreCase));
                Host.Delay(3111);
            Sanderling.WaitForMeasurement();
        }
    }
            if ( 0 < ListWreckOverviewEntry.Length && ListWreckOverviewEntry?.FirstOrDefault().DistanceMax > 60000)
    {
        ClickMenuEntryOnMenuRoot(ListWreckOverviewEntry?.FirstOrDefault(), "approach");
        Messages("out of range");//to add something if need
    }
    if (AnyDroneIdle && 0 < ListWreckOverviewEntry.Length)
        Sanderling.KeyboardPress(attackDrones);
    if (!(ListWreckOverviewEntry.Length > 0 ))
    {
        Messages("               no wrecks to salvage with drones, Finish the site");
        DroneEnsureInBay();
        return FinishingAsite;
    }
    return DronesAreSalvagingStep;
}
////


Func<object> SalvagingStep()
{
    Host.Log("               Salvaging step");
    var moduleSalvagerInactive = SetModuleSalvagerInactive?.FirstOrDefault();

    if (BackOnMainStep)
        return null;
    if (ListWreckOverviewEntry.Length == 0 || ListWreckOverviewEntry?.FirstOrDefault()?.DistanceMax > salvageRange)
    {
                Host.Log("               time to recover mtu");
        while (listOverviewMtu?.Length >0 )
        {
            var LootButton = Measurement?.WindowInventory?[0]?.ButtonText?.FirstOrDefault(text => text.Text.RegexMatchSuccessIgnoreCase("Loot All")); 
            if (LootButton != null)
                {
                Sanderling.MouseClickLeft(LootButton);
                Host.Log("               loot mtu :))");
                return FinishingAsite;
                }
            if ( listOverviewMtu?.FirstOrDefault()?.DistanceMax > 0)
                ClickMenuEntryOnMenuRoot(listOverviewMtu?.FirstOrDefault(), "open cargo");
        }
            Messages("               no wrecks to salvage with salvagers, Finish the site");
        return FinishingAsite;
    }
    if (null == moduleSalvagerInactive)
    {
            Host.Delay(1111);
        return SalvagingStep;
    }
    var setTargetWreckInRange   =
        SetTargetWreck?.Where(target => target?.DistanceMax <= salvageRange)?.ToArray();

    var wreckOverviewEntryNextNotTargeted = ListWreckOverviewEntry?.Where(entry => !((entry?.MeTargeted ?? false) || (entry?.MeTargeting ?? false))).ToArray();
    if (wreckOverviewEntryNextNotTargeted?.FirstOrDefault()?.DistanceMax > salvageRange)
    {
        Messages("out of range");//to add something if need
        ClickMenuEntryOnMenuRoot(wreckOverviewEntryNextNotTargeted?.FirstOrDefault(), "approach");

    }
    else
    {
    var virtualtargets =(!Measurement?.Target?.IsNullOrEmpty() ?? false ) ? Measurement?.Target?.Length:0;
    Host.Log(" "+Measurement?.Target?.Length+ " ; " +SetModuleSalvagerInactive?.Length + "   ; " +virtualtargets );
    if (  (Measurement?.Target?.IsNullOrEmpty() ?? false) || (virtualtargets <5 && !(ListWreckOverviewEntry?.IsNullOrEmpty() ?? false)))
            for (int i = 0; i < SetModuleSalvagerInactive?.Length ; i++)
           {
           virtualtargets =(!Measurement?.Target?.IsNullOrEmpty() ?? false ) ? Measurement?.Target?.Length:0;
           if (virtualtargets == SetModuleSalvagerInactive?.Length)
           break;
            Sanderling.KeyDown(lockTargetKeyCode);
            Sanderling.MouseClickLeft(wreckOverviewEntryNextNotTargeted.ElementAtOrDefault(i));
            Sanderling.KeyUp(lockTargetKeyCode);  
           continue;
           }
    }

    var setTargetWreckInRangeNotAssigned =
        setTargetWreckInRange?.Where(target => !(0 < target?.Assigned?.Length))?.ToArray();
    if(SetModuleSalvagerInactive?.Length >0  && setTargetWreckInRangeNotAssigned?.Length >0)
    {
    for (int i = 0; i <SetModuleSalvagerInactive?.Length ; i++)
        {
        setTargetWreckInRangeNotAssigned =
            setTargetWreckInRange?.Where(target => !(0 < target?.Assigned?.Length))?.ToArray();
        if (setTargetWreckInRangeNotAssigned?.IsNullOrEmpty() ?? false)
        break;
        Sanderling.MouseClickLeft(setTargetWreckInRangeNotAssigned?.ElementAtOrDefault(i));
        ModuleToggle(SetModuleSalvagerInactive?.ElementAtOrDefault(i));
        continue;
        }
    return SalvagingStep;
    }
    var wreckOverviewEntryNext = ListWreckOverviewEntry?.FirstOrDefault();


    if(null == wreckOverviewEntryNext || null == wreckOverviewEntryNextNotTargeted)
    {
        Host.Log("   no more wrecks??    ");
        return SalvagingStep;
    }

    return SalvagingStep;
}
///

Func<object> FinishingAsite()
{
        Host.Log("   Finishing a site    ");
    if (DronesInSpaceCount>0)
        DroneEnsureInBay();
    if (BackOnMainStep)
        return null;
    else
    {

    new System.Media.SoundPlayer(@"C:\boom_headshot.wav").Play();
    ActivateArmorExecute();
    deleteBookmark ();

    if (listOverviewMtu?.Length >0)
        RecoverMtu ();
    else
        MeChangeSalvage = false;
    KaboonusTalk();
    ++sitescount;
    if (OreHoldCapacityMilli?.Used > 0 )
        SiteFinished = true;
    else
        return TakeAnomaly;
    Messages("               Site Finished    " +SiteFinished);
    }
    return MainStep;
}
////////////////////
/////////////////////
IEnumerable<Parse.IMenu> Menu => Measurement?.Menu;
Parse.IShipUi ShipUi => Measurement?.ShipUi;

Sanderling.Parse.IMemoryMeasurement Measurement =>
    Sanderling?.MemoryMeasurementParsed?.Value;
IWindow ModalUIElement =>
    Measurement?.EnumerateReferencedUIElementTransitive()?.OfType<IWindow>()?.Where(window => window?.isModal ?? false)
    ?.OrderByDescending(window => window?.InTreeIndex ?? int.MinValue)
    ?.FirstOrDefault();
Sanderling.Interface.MemoryStruct.IMenuEntry MenuEntryLockTarget =>
    Menu?.FirstOrDefault()?.Entry?.FirstOrDefault(entry => entry.Text.RegexMatchSuccessIgnoreCase("^lock"));
Sanderling.Interface.MemoryStruct.IMenuEntry MenuEntryUnLockTarget =>
    Menu?.FirstOrDefault()?.Entry?.FirstOrDefault(entry => entry.Text.RegexMatchSuccessIgnoreCase("^unlock"));
Sanderling.Parse.IWindowOverview WindowOverview =>
    Measurement?.WindowOverview?.FirstOrDefault();
Sanderling.Parse.IWindowInventory WindowInventory =>
    Measurement?.WindowInventory?.FirstOrDefault();
IWindowDroneView WindowDrones =>
    Measurement?.WindowDroneView?.FirstOrDefault();
Tab OverviewTabActive =>
	Measurement?.WindowOverview?.FirstOrDefault()?.PresetTab
	?.OrderByDescending(tab => tab?.LabelColorOpacityMilli ?? 1500)
	?.FirstOrDefault();
Tab combatTab => WindowOverview?.PresetTab
	?.OrderByDescending(tab => tab?.Label.Text.RegexMatchSuccessIgnoreCase(rattingTab))
	?.FirstOrDefault();
Tab salvageTab => WindowOverview?.PresetTab
	?.OrderByDescending(tab => tab?.Label.Text.RegexMatchSuccessIgnoreCase(salvagingTab))
	?.FirstOrDefault();
var inventoryActiveShip = WindowInventory?.ActiveShipEntry;
var inventoryActiveShipEntry = WindowInventory?.ActiveShipEntry;

var RattingShip = WindowInventory?.LeftTreeListEntry?.SelectMany(entry => new[] { entry }.Concat(entry.EnumerateChildNodeTransitive()))
        ?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(RattingShipName) ?? false);
var SalvageShip = WindowInventory?.LeftTreeListEntry?.SelectMany(entry => new[] { entry }.Concat(entry.EnumerateChildNodeTransitive()))
        ?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(SalvageShipName) ?? false);

ITreeViewEntry InventoryActiveShipContainer =>
        WindowInventory?.ActiveShipEntry?.TreeEntryFromCargoSpaceType(ShipCargoSpaceTypeEnum.General);

IInventoryCapacityGauge OreHoldCapacityMilli =>
    (InventoryActiveShipContainer?.IsSelected ?? false) ? WindowInventory?.SelectedRightInventoryCapacityMilli : null;
int? OreHoldFillPercent => OreHoldCapacityMilli?.Max > 0 ? ((int?)((OreHoldCapacityMilli?.Used * 100) / OreHoldCapacityMilli?.Max )) : 0 ;


Sanderling.Accumulation.IShipUiModule[] ModuleWeapon =>
	Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Where(module => (module?.TooltipLast?.Value?.IsWeapon ?? false) || (module?.TooltipLast?.Value?.IsWeapon?? false))?.ToArray();
string OverviewTypeSelectionName =>
    WindowOverview?.Caption?.RegexMatchIfSuccess(@"\(([^\)]*)\)")?.Groups?[1]?.Value;

Parse.IOverviewEntry[] FrigateListRatOverviewEntry => WindowOverview?.ListView?.Entry?.Where(entry =>
   ( (entry?.MainIconIsRed ?? false)
   &&( entry?.Name?.RegexMatchSuccessIgnoreCase(@"coreli|centi|alvi|pithi|corpii|gistii|cleric|engraver|corelum
   |battery|tower|sentry|web|strain|splinter|render|raider|friar|reaver|guardian|loyal|dire|elder|arch") ?? false)
    ))
    ?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
    ?.ToArray();

Parse.IOverviewEntry[] ListRatOverviewEntry => WindowOverview?.ListView?.Entry?.Where(entry =>
    (entry?.MainIconIsRed ?? false))
    ?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
    ?.ToArray();

Parse.IOverviewEntry[] listOverviewCommanderWreck =>
	WindowOverview?.ListView?.Entry
	?.Where(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(commanderNameWreck) ?? true)
	?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
    .ToArray();
Parse.IOverviewEntry[] ListCelestialObjects => WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(celestialOrbit) ?? false)
    ?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
    ?.ToArray();
Parse.IOverviewEntry[] ListCelestialToAvoid => WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(CelestialToAvoid ) ?? false)
    ?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
    ?.ToArray();
Parse.IOverviewEntry[] listOverviewDreadCheck => WindowOverview?.ListView?.Entry
    ?.Where(entry => (entry?.Name?.RegexMatchSuccess(runFromRats) ?? true))
    .ToArray();
Parse.IOverviewEntry[] listOverviewEntryFriends =>
    WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.ListBackgroundColor?.Any(IsFriendBackgroundColor) ?? false)
    ?.ToArray();
Parse.IOverviewEntry[] listOverviewEntryEnemy =>
    WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.ListBackgroundColor?.Any(IsEnemyBackgroundColor) ?? false)
    ?.ToArray();
Parse.IOverviewEntry[] listOverviewMtu => WindowOverview?.ListView?.Entry
    ?.Where(mtu =>  (mtu?.Name?.RegexMatchSuccessIgnoreCase(MTUName) ?? true) 
        && (mtu?.ListColumnCellLabel?.FirstOrDefault(corp =>corp.Value?.RegexMatchSuccessIgnoreCase(MyCorpo) ?? false ).Value?.RegexMatchSuccessIgnoreCase(MyCorpo) ?? true)  )
    .ToArray();
Sanderling.Accumulation.IShipUiModule[] SetModuleSalvager =>
    Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Where(module => module?.TooltipLast?.Value?.LabelText?.Any(
    label => label?.Text?.RegexMatchSuccess(ModuleSalvagerX, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false) ?? false)?.ToArray();	

Sanderling.Accumulation.IShipUiModule[] SetModuleSalvagerInactive	 =>
	SetModuleSalvager?.Where(module => !(module?.RampActive ?? false))?.ToArray();
Sanderling.Accumulation.IShipUiModule ModuleShieldBooster =>
    Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.FirstOrDefault(module => module?.TooltipLast?.Value?.IsShieldBooster ?? false);
Sanderling.Accumulation.IShipUiModule[] ModuleArmorRepairer =>
    Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Where(module => (module?.TooltipLast?.Value?.IsArmorRepairer ?? false)
    || (module?.TooltipLast?.Value?.LabelText?.Any(label => label?.Text?.RegexMatchSuccess(UnsupportedArmorRepairer, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false) ?? false))
    ?.ToArray();
Sanderling.Accumulation.IShipUiModule ModuleAfterburner =>
    Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.FirstOrDefault(module => (module?.TooltipLast?.Value?.IsAfterburner ?? false)
    || (module?.TooltipLast?.Value?.IsMicroWarpDrive?? false));

Sanderling.Accumulation.IShipUiModule[] ModulePerma =>
		Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Where(module => (module?.TooltipLast?.Value?.LabelText?.Any(
		label => label?.Text?.RegexMatchSuccess(permanentactive,System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false )?? false )|| ( module?.TooltipLast?.Value?.IsHardener ?? false))
        ?.ToArray();

Sanderling.Parse.IShipUiTarget[] SetTargetWreck =>	Measurement?.Target?.Where(target =>
		target?.TextRow?.Any(textRow => textRow.RegexMatchSuccessIgnoreCase("wreck")) ?? false)?.ToArray();

Parse.IOverviewEntry[] ListWreckOverviewEntry =>
	WindowOverview?.ListView?.Entry
	?.Where(entry => entry.Name.RegexMatchSuccessIgnoreCase("wreck"))
	?.OrderBy(entry => entry.DistanceMax ?? int.MaxValue)
	?.ToArray();
Parse.IOverviewEntry[] EWarToAttack =>
    WindowOverview?.ListView?.Entry
    ?.Where(entry => entry != null && (!entry?.EWarType?.IsNullOrEmpty() ?? false) && (entry?.EWarType).Any())
    //?.Where(entry => entry.EWarType.Any())
    ?.OrderBy(ewar => EWarTypeEnum.Web)
    ?.OrderBy(ewar => EWarTypeEnum.WarpScramble)
    ?.OrderBy(ewar => EWarTypeEnum.WarpDisrupt)
    ?.OrderBy(ewar => EWarTypeEnum.EnergyNeut)
    ?.OrderBy(ewar => EWarTypeEnum.EnergyVampire)
    ?.OrderBy(ewar => EWarTypeEnum.ECM)
    ?.OrderBy(ewar => EWarTypeEnum.SensorDamp)
    ?.OrderBy(ewar => EWarTypeEnum.TrackingDisrupt)
    ?.OrderBy(ewar => EWarTypeEnum.TargetPaint)
    ?.OrderBy(ewar => EWarTypeEnum.Miscellaneous)
    ?.OrderBy(ewar => EWarTypeEnum.Other)
    ?.OrderBy(ewar => EWarTypeEnum.None)
	?.ToArray();

Parse.IOverviewEntry[] listOverviewStationName =>
    WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(StationHomeName) ?? true)
    .ToArray();
DroneViewEntryGroup DronesInBayListEntry =>
    WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryGroup>()?.FirstOrDefault(Entry => null != Entry?.Caption?.Text?.RegexMatchIfSuccess(@"Drones in bay", RegexOptions.IgnoreCase));
DroneViewEntryGroup DronesInSpaceListEntry =>
    WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryGroup>()?.FirstOrDefault(Entry => null != Entry?.Caption?.Text?.RegexMatchIfSuccess(@"Drones in Local Space", RegexOptions.IgnoreCase));

DroneViewEntryGroup DronesInBayListFolderEntry =>
    WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryGroup>()?.FirstOrDefault(Entry => null != Entry?.Caption?.Text?.RegexMatchIfSuccess(CombatDronesFolder, RegexOptions.IgnoreCase));
DroneViewEntryGroup DronesInSpaceListFolderEntry =>
    WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryGroup>()?.LastOrDefault(Entry => null != Entry?.Caption?.Text?.RegexMatchIfSuccess(CombatDronesFolder, RegexOptions.IgnoreCase));
DroneViewEntryGroup DronesInSpaceNoItem=>
    WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryGroup>()?.LastOrDefault(Entry => null != Entry?.Caption?.Text?.RegexMatchIfSuccess("No Item", RegexOptions.IgnoreCase));

DroneViewEntryGroup DronesInBayListFolderSalvageEntry =>
    WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryGroup>()?.FirstOrDefault(Entry => null != Entry?.Caption?.Text?.RegexMatchIfSuccess(SalvageDronesFolder, RegexOptions.IgnoreCase));

DroneViewEntryItem[] AllDrones => WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryItem>()?.ToArray();

int?		WeaponRange => ModuleWeapon?.Select(module =>
	module?.TooltipLast?.Value?.RangeOptimal ?? module?.TooltipLast?.Value?.RangeMax ?? module?.TooltipLast?.Value?.RangeWithin  ?? 0)?.DefaultIfEmpty(0)?.Min();
int? RangeMissiles => DistanceMinFromLabelWithRegexPattern(@"Max flight range<br>\s*").HasValue ? DistanceMinFromLabelWithRegexPattern(@"Max flight range<br>\s*") : RangeMissilesDefault;
int?		WeaponMaxRange => Math.Max(RangeMissiles.Value, WeaponRange.Value);
int MySpeed => Measurement?.ShipUi?.SpeedMilli.HasValue ?? false  ?(int)Sanderling?.MemoryMeasurementParsed?.Value?.ShipUi?.SpeedMilli.Value /1000 : 0;

int? DronesInSpaceCount => DronesInSpaceListEntry?.Caption?.Text?.AsDroneLabel()?.Status?.TryParseInt();
int? DronesInBayCount => DronesInBayListEntry?.Caption?.Text?.AsDroneLabel()?.Status?.TryParseInt();
int? DronesInBaySalvageFolderCount =>DronesInBayListFolderSalvageEntry != null ? ((int?) (DronesInBayListFolderSalvageEntry?.Caption?.Text?.AsDroneLabel()?.Status?.TryParseInt())): 0;
int? DronesInBayCombatFolderCount =>DronesInBayListFolderEntry != null ? ((int?) (DronesInBayListFolderEntry?.Caption?.Text?.AsDroneLabel()?.Status?.TryParseInt())): 0;
int? DronesInSpaceCombatFolderCount =>DronesInSpaceNoItem != null ? ((int?) (DronesInSpaceListFolderEntry?.Caption?.Text?.AsDroneLabel()?.Status?.TryParseInt())) : 0;
int? DronesInBayFromFoldersCount => DronesInBaySalvageFolderCount + DronesInBayCombatFolderCount;

public bool Tethering =>
    Measurement?.ShipUi?.EWarElement?.Any(EwarElement => (EwarElement?.EWarType).RegexMatchSuccess("tethering")) ?? false;
public bool Aligning =>
    Measurement?.ShipUi?.Indication?.LabelText?.Any(indicationLabel =>
        (indicationLabel?.Text).RegexMatchSuccessIgnoreCase("aligning")) ?? false;
public bool ReadyForManeuverNot =>
    Measurement?.ShipUi?.Indication?.LabelText?.Any(indicationLabel =>
        (indicationLabel?.Text).RegexMatchSuccessIgnoreCase("warp|docking")) ?? false;
public bool Dockinginstance =>
    Measurement?.ShipUi?.Indication?.LabelText?.Any(indicationLabel =>
        (indicationLabel?.Text).RegexMatchSuccessIgnoreCase("docking")) ?? false;
public bool SpeedWarping => Measurement?.ShipUi?.SpeedLabel?.Text?.RegexMatchSuccessIgnoreCase("(Warping)") ?? false;
public bool EmptyIndication =>
    Measurement?.ShipUi?.Indication?.LabelText?.Any(indicationLabel =>
        (indicationLabel?.Text).RegexMatchSuccessIgnoreCase("")) ?? false;
public bool ShipIsSleeping => (EmptyIndication || !(MySpeed>2));
public bool ReadyForManeuver => !Dockinginstance  && !(Measurement?.IsDocked ?? true);
public bool ReadyToBattle => 0 < ListRatOverviewEntry?.Length && ReadyForManeuver;
public bool NoRatsOnGrid => 0 == ListRatOverviewEntry?.Length || ListRatOverviewEntry?.FirstOrDefault()?.DistanceMax > MaxDistanceToRats;
public bool LookingAtStars => NoRatsOnGrid && ReadyForManeuver;
var reasonDrones = false;
int L=3;
string permanentactive ;
/////
Sanderling.Interface.MemoryStruct.IListEntry WindowInventoryItem =>
    WindowInventory?.SelectedRightInventory?.ListView?.Entry?.FirstOrDefault();
WindowChatChannel chatLocal =>
     Sanderling.MemoryMeasurementParsed?.Value?.WindowChatChannel
     ?.FirstOrDefault(windowChat => windowChat?.Caption?.RegexMatchSuccessIgnoreCase("local") ?? false);
//

int ? IgnoreListCount  => chatLocal?.ParticipantView?.Entry?.Where(ignoreplayer =>ignoreplayer?.NameLabel?.Text?.RegexMatchSuccessIgnoreCase(IgnoreNeutral) ?? false).ToArray()?.Count() ;
public bool YesHostiles => 1+IgnoreListCount < chatLocal?.ParticipantView?.Entry?.Count(IsNeutralOrEnemy);
//
var logoutme= false;
var logoutgame = (eveRealServerDT-DateTime.UtcNow ).TotalMinutes;
bool ReasonCapsuled => WindowInventory?.ActiveShipEntry?.Text?.RegexMatchSuccessIgnoreCase(@"capsule") ?? false;
bool ReasonTimeElapsed => (logoutme);
bool ReasonDrones => (reasonDrones && activeshipnameRatting);
bool ReasonCargoFull => (OreHoldFilledForOffload || FullCargoMessage);
bool ReasonDread=> (listOverviewDreadCheck?.Length > 0);
string Preparatus;
ITreeViewEntry InventoryActiveShipDronesContainer =>
        WindowInventory?.ActiveShipEntry?.TreeEntryFromCargoSpaceType(ShipCargoSpaceTypeEnum.DroneBay);
IInventoryCapacityGauge DronesHoldCapacityMilli =>
    (InventoryActiveShipDronesContainer?.IsSelected ?? false) ? WindowInventory?.SelectedRightInventoryCapacityMilli : null;
int? DronesHoldFillPercent =>DronesHoldCapacityMilli?.Max > 0 ? ((int?)((DronesHoldCapacityMilli?.Used * 100) / DronesHoldCapacityMilli?.Max )) : 0 ;

bool? IsExpanded(IInventoryTreeViewEntryShip shipEntryInInventory) =>
	shipEntryInInventory == null ? null :
	(bool?)((shipEntryInInventory.IsExpanded ?? false) || 0 < (shipEntryInInventory.Child?.Count() ?? 0));
string InventoryContainerLabelRegexPatternFromContainerName(string containerName) =>
    @"^\s*" + Regex.Escape(containerName) + @"\s*($|\<)";
public bool StationProximity => listOverviewStationName?.FirstOrDefault()?.DistanceMin < 10000;
public bool BackOnMainStep => (!ReadyForManeuver) || Tethering || (Measurement?.IsDocked ?? false) || StationProximity || null != RetreatReason;
//////////////


void FlashWindowProbes()
{
    Sanderling.KeyboardPressCombined(new[] { VirtualKeyCode.LMENU, VirtualKeyCode.VK_P });
}

void ReviewSettings()
{
    LogMessageToFile(" Review: "+VersionScript+ "/" +CharName+ "/" +CurrentSystem+ " # Sites : " + sitescount+ "; Total: " +MagicalPrepare+ " ISK # Session: " +Paracelsus+ " ISK ; "   +killedratscompared+ " killedrats  .  " );
        Host.Log("                >>> Taped into file.");
}

void Messages (string text )
{

  Host.Log(text);
CurrentMessage =text;
}

void CloseModalUIElement()
{

    var ConnectionLost = Measurement?.WindowOther?.FirstOrDefault()?.LabelText?.FirstOrDefault(text => (text?.Text.RegexMatchSuccessIgnoreCase("Connection") ?? false));
    var NotEnoughCargo = Sanderling?.MemoryMeasurementParsed?.Value?.WindowOther?.FirstOrDefault()?.LabelText?.FirstOrDefault(text => (text?.Text.RegexMatchSuccessIgnoreCase("Not enough cargo space") ?? false));
    var ButtonClose =
        ModalUIElement?.ButtonText?.FirstOrDefault(button => (button?.Text).RegexMatchSuccessIgnoreCase("quit|close|no|ok"));
    var ButtonQuit =
        ModalUIElement?.ButtonText?.FirstOrDefault(button => (button?.Text).RegexMatchSuccessIgnoreCase("quit|close|ok"));
     if (ConnectionLost != null)
        {
            Messages("               Lost connection (modal) at : " + DateTime.Now.ToString(" HH:mm")+ "" );
            new System.Media.SoundPlayer(@"C:\disturbance_obi_wan.wav").Play();
            Host.Delay(1111);
            Sanderling?.KillEveProcess();
            BotStopActivity();
        }
        if (ButtonQuit != null)
            Sanderling.MouseClickLeft(ButtonQuit);

    if (NotEnoughCargo != null)
    {
        var OkyButton = Sanderling?.MemoryMeasurementParsed?.Value?.WindowOther?.FirstOrDefault()?.ButtonText?.FirstOrDefault(text => text.Text.RegexMatchSuccessIgnoreCase("ok"));
        if (OkyButton != null)
            Sanderling.MouseClickLeft(OkyButton);
        Host.Delay(3500);
            FullCargoMessage = true;
        ModuleStopToggle(ModuleAfterburner);
            ActivateArmorExecute();
        ModuleToggle(ModuleShieldBooster);
            WarpingSlow(RetreatBookmark, "dock");
    }
    else
        Sanderling.MouseClickLeft(ButtonClose);
}
void CloseWindowTelecom()
{

    var WindowTelecom = Measurement?.WindowTelecom?.FirstOrDefault(w => (w?.Caption.RegexMatchSuccessIgnoreCase("Information|Expeditions") ?? false));
    var CloseButton = WindowTelecom?.ButtonText?.FirstOrDefault(text => text.Text.RegexMatchSuccessIgnoreCase("Close"));
    var OKButton = WindowTelecom?.ButtonText?.FirstOrDefault(text => text.Text.RegexMatchSuccessIgnoreCase("ok"));
    var HavenTelecom = Measurement?.WindowTelecom?.FirstOrDefault()?.LabelText?.FirstOrDefault(text => (text?.Text.RegexMatchSuccessIgnoreCase("Ship Computer") ?? false));
    if (CloseButton != null)
        Sanderling.MouseClickLeft(CloseButton);
    if (OKButton != null)
        Sanderling.MouseClickLeft(OKButton);
}
public void CloseWindowOther()
{

     var ButtonQuit =
        ModalUIElement?.ButtonText?.FirstOrDefault(button => (button?.Text).RegexMatchSuccessIgnoreCase("quit|close|no|ok"));
    var windowOther = Sanderling?.MemoryMeasurementParsed?.Value?.WindowOther?.FirstOrDefault();
    if (ButtonQuit != null)
        Sanderling.MouseClickLeft(ButtonQuit);
    if (!windowOther?.HeaderButtonsVisible ?? false)
        Sanderling.MouseMove(windowOther.LabelText.FirstOrDefault());

    Sanderling.InvalidateMeasurement();
    if (windowOther?.HeaderButton != null)
    {
        var closeButton = windowOther.HeaderButton?.FirstOrDefault(x => x.HintText == "Close");
        if (closeButton != null)
            Sanderling.MouseClickLeft(closeButton);
    }
}

void MemoryUpdate()
{
 	RetreatUpdate();
	Timers (); 
}

void RetreatUpdate()
{

    if (YesHostiles	|| (listOverviewEntryEnemy?.Length > 0)|| tooManyOnLocal)
    {
        Sanderling.InvalidateMeasurement();
        Sanderling.WaitForMeasurement();
        if ( YesHostiles || (listOverviewEntryEnemy?.Length > 0)|| tooManyOnLocal)
            RetreatReasonHostiles = " Hostiles or too many in local ! ";
    }
    else RetreatReasonHostiles = null;
        SiteFinishRetreat = SiteFinished ? " Site Finished": null;
    RetreatReasonArmor = (!(Measurement?.IsDocked ?? false) &&!Tethering && (!(EmergencyWarpOutHitpointPercent < ArmorHpPercent)))? " They messed my Armor hp!!" : null;
        RetreatReasonDrones = ReasonDrones ? " I lost my head ( Drones)!!" : null;
    RetreatReasonCargoFull = ReasonCargoFull ? " Cargo Full !!" : null;
        ChangingToSalvager = (MeChangeSalvage) ? " Change to Salvage !!" : null;
    RetreatReasonBumped = (0 <= ListRatOverviewEntry?.Length) && (listOverviewEntryFriends?.Length > 0) && (listOverviewEntryFriends?.FirstOrDefault()?.DistanceMax < 250) ?" Retreat: I was bumped !!" : null;
        RetreatReasonCapsuled = ReasonCapsuled ? " Retreat: Capsuled, go home" : null;
    RetreatReasonTimeElapsed = logoutme ? " Retreat: Your session elapsed, take a break!" : null;
            if (ReasonDread)
    {
    Sanderling.InvalidateMeasurement();
    Sanderling.WaitForMeasurement();
    if (ReasonDread)
        RetreatReasonDread = " Retreat!! Dread on Grid!!";
    }
    else RetreatReasonDread = null;
}

void Timers ()
{

    var now = DateTime.UtcNow;
    var CloseGameSession = (playSession - now).TotalMinutes;
    var CloseGameDT = (eveSafeDT - now).TotalMinutes;
    var LogoutGame = Math.Min(CloseGameDT,CloseGameSession);
    if (playSession !=DateTime.UtcNow)
        logoutgame = LogoutGame;
	if (LogoutGame < 0) 
	{
	    logoutme = true;
		Messages("               Logoutgame is" + logoutme + " ");
	}
}


void CountingRatsUpdate()
{

    if ( !ReadyForManeuver || null != RetreatReason )
        return;
	var	NumberRatsSynced	= ListRatOverviewEntry?.Count();
	if(NumberRatsSynced < LastCheckNumberRatsSynced || ( LastCheckNumberRatsSynced ==1 &&( NumberRatsSynced == 0 || NumberRatsSynced > LastCheckNumberRatsSynced ) ))
		++killedratscompared;
	LastCheckNumberRatsSynced = NumberRatsSynced;
}

void EnsureWindowInventoryOpen()
{
    if (null != WindowInventory)
        return;
    Sanderling.MouseClickLeft(Measurement?.Neocom?.InventoryButton);
    Host.Delay(31111);
}
void EnsureWindowInventoryOpenActiveShip()
{
    EnsureWindowInventoryOpen();
    var inventoryActiveShip = WindowInventory?.ActiveShipEntry;
    if (!(inventoryActiveShip?.IsSelected ?? false))
		Sanderling.MouseClickLeft(inventoryActiveShip);
}

void WarpInstant ()
{
    K=1;
    if (0 < DronesInSpaceCount)
        DroneEnsureInBay();
    else if (listOverviewStationName?.Length > 0 )
    {
            Console.Beep(1500, 200);
        Sanderling.KeyDown(Warpkey);
            Sanderling.MouseClickLeft(listOverviewStationName?.FirstOrDefault());
        Sanderling.KeyUp(Warpkey);
        Messages("               Try warp  Instant    " +listOverviewStationName?.FirstOrDefault()?.Name+ "");
        Host.Delay(444);
    }
    else if (listOverviewStationName?.Length == 0 )
        WarpingSlow(RetreatBookmark, "dock");
}

void WarpingSlow(string destination, string action)
{
    K=1;
    ClickMenuEntryOnPatternMenuRoot(Sanderling?.MemoryMeasurementParsed?.Value?.InfoPanelCurrentSystem?.ListSurroundingsButton, destination, action);
}

void Expander(string Label)
{
    	var Element = Measurement?.WindowDroneView?.FirstOrDefault()?.ListView?.Entry?.FirstOrDefault(w => w?.LabelText?.FirstOrDefault()?.Text?.RegexMatchSuccessIgnoreCase("^" + Label) ?? false);
    bool IsExpanded = Element?.IsExpanded ?? true;
    if(!IsExpanded)
        ClickMenuEntryOnMenuRoot(Element,"Expand");
    var SpaceElement = Measurement?.WindowDroneView?.FirstOrDefault()?.ListView?.Entry?.LastOrDefault(w => w?.LabelText?.FirstOrDefault()?.Text?.RegexMatchSuccessIgnoreCase("^" + Label) ?? false);
	bool IsSpaceExpanded = SpaceElement?.IsExpanded ?? true;
	if(!IsSpaceExpanded) ClickMenuEntryOnMenuRoot(SpaceElement,"Expand");
}

void Collapse(string Label)
{
	    var Element = Measurement?.WindowDroneView?.FirstOrDefault()?.ListView?.Entry?.FirstOrDefault(w => w?.LabelText?.FirstOrDefault()?.Text?.RegexMatchSuccessIgnoreCase("^" + Label) ?? false);
    bool IsExpanded = !(Element?.IsExpanded ?? true);
    if(!IsExpanded)
        ClickMenuEntryOnMenuRoot(Element,"Collapse");
}

void DroneLaunch()
{
        Host.Log("                void  Launching my Vipers");
    Expander("Drones in Bay");
    if (DronesInBayListFolderEntry != null)
    {
        Sanderling.MouseClickRight(DronesInBayListFolderEntry);
        Sanderling.MouseClickLeft(Menu?.FirstOrDefault()?.EntryFirstMatchingRegexPattern("launch drones", RegexOptions.IgnoreCase));
    }
    else
    {
        Sanderling.MouseClickRight(DronesInBayListEntry);
        Sanderling.MouseClickLeft(Menu?.FirstOrDefault()?.EntryFirstMatchingRegexPattern("launch drones", RegexOptions.IgnoreCase));
    }
    Messages("               Launching my Vipers");
        Host.Delay(1444);
    Expander(CombatDronesFolder);
}

void DroneEnsureInBay()
{
    if (null == WindowDrones || DronesInSpaceCount==0)
        return;
     else
    DroneReturnToBay();

}

void DroneReturnToBay()
{
    Host.Log("               I do not forget my Vipers here");

     Sanderling.KeyboardPressCombined(new[]{ targetLockedKeyCode, VirtualKeyCode.VK_R });
    Host.Delay(544);
}

void ClickMenuEntryOnMenuRoot(IUIElement MenuRoot, string MenuEntryRegexPattern)
{
    Sanderling.MouseClickRight(MenuRoot);
    var Menu = Measurement?.Menu?.FirstOrDefault();
    var MenuEntry = Menu?.EntryFirstMatchingRegexPattern(MenuEntryRegexPattern, RegexOptions.IgnoreCase);
    Sanderling.MouseClickLeft(MenuEntry);
}

void ClickMenuEntryOnPatternMenuRoot(IUIElement MenuRoot, string MenuEntryRegexPattern, string SubMenuEntryRegexPattern = null)
{
    Sanderling.MouseClickRight(MenuRoot);
    var Menu = Sanderling?.MemoryMeasurementParsed?.Value?.Menu?.FirstOrDefault();
    var MenuEntry = Menu?.EntryFirstMatchingRegexPattern(MenuEntryRegexPattern, RegexOptions.IgnoreCase);
    Sanderling.MouseClickLeft(MenuEntry);
    if (SubMenuEntryRegexPattern != null)
    {
		var subMenu = Sanderling?.MemoryMeasurementParsed?.Value?.Menu?.ElementAtOrDefault(1);
        var subMenuEntry = subMenu?.EntryFirstMatchingRegexPattern(SubMenuEntryRegexPattern, RegexOptions.IgnoreCase);
        Sanderling.MouseClickLeft(subMenuEntry);
    }
}

void Orbitkeyboard()
{
    if (0 == ListCelestialObjects?.Length && combatTab == OverviewTabActive )
    {
        Sanderling.MouseClickLeft(salvageTab);
        Host.Delay(1111);
    }
    if (0 == ListCelestialObjects?.Length)
        OrbitRats();
    if (1 == ListCelestialObjects?.Length)
    {
        Sanderling.KeyDown(orbitKeyCode);
        Sanderling.MouseClickLeft(ListCelestialObjects?.FirstOrDefault());
        Sanderling.KeyUp(orbitKeyCode);
    }
    else if (0 == ListCelestialObjects?.Count(crystal =>crystal?.DistanceMax > DistanceCelestial))
    {
        Sanderling.KeyDown(orbitKeyCode);
        Sanderling.MouseClickLeft(ListCelestialObjects?.OrderByDescending(celestial => celestial?.DistanceMax ?? int.MaxValue)?.FirstOrDefault());
        Sanderling.KeyUp(orbitKeyCode);
    }
    else if (0 < ListCelestialObjects?.Count(crystal =>crystal?.DistanceMax > DistanceCelestial))
    {
        Sanderling.KeyDown(orbitKeyCode);
        Sanderling.MouseClickLeft(ListCelestialObjects?.FirstOrDefault(celestial => celestial?.DistanceMax > DistanceCelestial));
        Sanderling.KeyUp(orbitKeyCode);
    }
    ActivateAfterburnerExecute();
    Host.Delay(1111);
    Messages("               Orbit arround Celestials");
}
void OrbitRats()
{
    Sanderling.KeyDown(orbitKeyCode);
         Sanderling.MouseClickLeft(ListRatOverviewEntry?.FirstOrDefault(entry => (entry?.MainIconIsRed ?? false))); 
    Sanderling.KeyUp(orbitKeyCode);
    ActivateAfterburnerExecute();
    Host.Delay(1111);
    Messages("               Rats are too far ... selected to Orbit them");
}

void ModuleToggle(Sanderling.Accumulation.IShipUiModule Module)
{
    var ToggleKey = Module?.TooltipLast?.Value?.ToggleKey;
    Host.Log("               Toggle module  '" +Module?.TooltipLast?.Value?.LabelText?.ElementAtOrDefault(1)?.Text?.RemoveXmlTag() +      "'  using " + (null == ToggleKey ? "mouse" : Module?.TooltipLast?.Value?.ToggleKeyTextLabel?.Text));
    if (null == ToggleKey)
        Sanderling.MouseClickLeft(Module);
    else
        Sanderling.KeyboardPressCombined(ToggleKey);
}
void ModuleStopToggle(Sanderling.Accumulation.IShipUiModule Module)
{
    var ToggleKey = Module?.TooltipLast?.Value?.ToggleKey;
    var ActiveModuleToToggle = Module?.RampActive ?? false;
    if( ActiveModuleToToggle)
    {
        if (null == ToggleKey )
            Sanderling.MouseClickLeft(Module);
        else
            Sanderling.KeyboardPressCombined(ToggleKey);
        Host.Log("              Stop module  '" + Module?.TooltipLast?.Value?.LabelText?.ElementAtOrDefault(1)?.Text?.RemoveXmlTag() +      "'  using " + (null == ToggleKey ? "mouse" : Module?.TooltipLast?.Value?.ToggleKeyTextLabel?.Text));
    }
}

void LockTarget()
{
    Sanderling.KeyDown(lockTargetKeyCode);
    Sanderling.MouseClickLeft(ListRatOverviewEntry?.FirstOrDefault(entry => !((entry?.MeTargeted ?? false) || (entry?.MeTargeting ?? false))));
    Sanderling.KeyUp(lockTargetKeyCode);
}

void UnlockTarget()
{
    var targetSelected = Measurement?.Target?.FirstOrDefault(target => target?.IsSelected ?? false);
    Sanderling.MouseClickRight(targetSelected);
    Sanderling.MouseClickLeft(MenuEntryUnLockTarget);
    Host.Log("                this is not a target");
}

void ModuleMeasureAllTooltip()
{
    var moduleUnknownCount = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Count((module => null == module?.TooltipLast?.Value?.LabelText?.Any()));
    var initialmoduleCount = moduleUnknownCount;


    while( moduleUnknownCount >0	)
	{
		if(Dockinginstance || ( Measurement?.IsDocked ?? false))
			break;
        for (int i = 0; i < Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Count(); ++i)
		{
            var NextModule = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.ElementAtOrDefault(i);
			if(!ReadyForManeuver)
				break;
			if(null == NextModule)
				break;
			Sanderling.MouseMove(NextModule);
            Host.Delay(305);
			Sanderling.WaitForMeasurement();

                Host.Delay(305);
			Sanderling.MouseMove(NextModule);

            Host.Log("               R2D2 detected a new module: " +Measurement?.ModuleButtonTooltip?.LabelText?.ElementAtOrDefault(1)?.Text?.RemoveXmlTag() + "");
		}


        moduleUnknownCount = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Count((module => null == module?.TooltipLast?.Value?.LabelText?.Any()));
        Host.Log("               R2D2 updated counted modules from " + initialmoduleCount+ " to : " +moduleUnknownCount+"");

    }
}

void ActivatePainterExecute()
{

    var SubsetModulePainter = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
        ?.Where(module => (module?.TooltipLast?.Value?.IsTargetPainter ?? false) || (module?.TooltipLast?.Value?.IsTargetPainter?? false));
    var SubsetModuleToToggle =
        SubsetModulePainter
        ?.Where(module => !(module?.RampActive ?? false));
    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}

void ActivateArmorExecute()
{
var SubsetModuleArmor = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
    ?.Where(module => (module?.TooltipLast?.Value?.IsArmorRepairer ?? false) || (module?.TooltipLast?.Value?.IsShieldBooster ?? false) || (module?.TooltipLast?.Value?.LabelText?.Any(
            label => label?.Text?.RegexMatchSuccess(UnsupportedArmorRepairer, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false) ?? false)) ;

    var SubsetModuleToToggle =
        SubsetModuleArmor
        ?.Where(module => !(module?.RampActive ?? false));
    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}

 
void PermaExecute()
{


foreach (string permanentactive in PermanentActive)
{
    var SubsetModulePerma =
		Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Where(module => (module?.TooltipLast?.Value?.LabelText?.Any(
		label => label?.Text?.RegexMatchSuccess(permanentactive,System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false )?? false )|| ( module?.TooltipLast?.Value?.IsHardener ?? false));
        if  (SubsetModulePerma?.Count() == 0)
        break;
    var SubsetModuleToToggle =
        SubsetModulePerma
        ?.Where(module => !(module?.RampActive ?? false));
    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
        Host.Delay(311);
        }

}
void ActivateAfterburnerExecute()
{
var SubsetModuleAfterburner =
    Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
    ?.Where(module => (module?.TooltipLast?.Value?.IsAfterburner ?? false) || (module?.TooltipLast?.Value?.IsMicroWarpDrive?? false));
    var SubsetModuleToToggle =
        SubsetModuleAfterburner
        ?.Where(module => !(module?.RampActive ?? false));

    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}
void ShootWeapon()
{
    var SubsetModuleWeapon = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Where(module => (module?.TooltipLast?.Value?.IsWeapon ?? false) || (module?.TooltipLast?.Value?.IsWeapon?? false));
    var SubsetModuleToToggle =
        SubsetModuleWeapon
        ?.Where(module => !(module?.RampActive ?? false));
    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}

void CheckLocation()
{
        Host.Log("                # Checking for bookmarks ");
    Sanderling.MouseClickRight(Sanderling?.MemoryMeasurementParsed?.Value?.InfoPanelCurrentSystem?.ListSurroundingsButton);
    var	availableMenuEntriesTexts = Measurement?.Menu?.FirstOrDefault()?.Entry?.Select(menuEntry => menuEntry.Text)
            ?.ToList();
    Sanderling.WaitForMeasurement();
    var OldSiteMenuEntry = availableMenuEntriesTexts?.Where(x => x?.Contains(messageText) ?? false)?.FirstOrDefault();
    if (null != OldSiteMenuEntry)
        OldSiteExist = true;
    if (null == OldSiteMenuEntry )
        OldSiteExist = false;
        Messages("               OldSiteExist    :    " +OldSiteExist);

}




void SavingLocation ()
{
    if (OldSiteExist)
        Host.Log("                # Already have  Old Site Bookmark : "+OldSiteExist+ " . ");
    else
    {
        var SaveLocationWindow = Measurement?.WindowOther?.FirstOrDefault(w =>
                                (w?.Caption.RegexMatchSuccessIgnoreCase("New Location") ?? false));
        Sanderling.KeyboardPressCombined(new[]{ VirtualKeyCode.LCONTROL, VirtualKeyCode.VK_B});
            Host.Delay(1111);
        Sanderling.TextEntry(messageText);
            Host.Delay(1111);
        Sanderling.KeyboardPress(VirtualKeyCode.RETURN);
                Host.Delay(1111);
        OldSiteExist = true;
            Host.Log("                >> Old Site Bookmark saved : " +OldSiteExist+ " . ");
    }
}
void deleteBookmark()
{

        ClickMenuEntryOnPatternMenuRoot(Sanderling?.MemoryMeasurementParsed?.Value?.InfoPanelCurrentSystem?.ListSurroundingsButton, messageText, "Remove Location");
            Host.Delay(511);
        Sanderling.KeyboardPress(VirtualKeyCode.RETURN);
        CheckLocation();
            Host.Delay(1111);
        Messages("                # Old Site Bookmark removed : " +!OldSiteExist+ " . ");
}
void ReturnToOldSite ()
{
    CheckLocation();
    if (OldSiteExist )
    {
        Sanderling.MouseClickRight(Sanderling?.MemoryMeasurementParsed?.Value?.InfoPanelCurrentSystem?.ListSurroundingsButton);
        if (!(Measurement?.Menu?.FirstOrDefault()?.Entry?.Where(top => top?.Text?.RegexMatchSuccessIgnoreCase(messageText) ?? false)?.FirstOrDefault()?.HighlightVisible ?? false))
            Sanderling.MouseClickLeft(Measurement?.Menu?.FirstOrDefault()?.Entry?.Where(top => top?.Text?.RegexMatchSuccessIgnoreCase(messageText) ?? false)?.FirstOrDefault());
        var OldSiteApproach = Measurement?.Menu?.ToList() ? [1].Entry?.Where(x => x?.Text.RegexMatchSuccessIgnoreCase("approach") ?? false)?.FirstOrDefault();
        if (OldSiteApproach != null)
            ImOnSite = true;
        else
            ImOnSite = false;
        Messages("                # Warping to Old Site : "+OldSiteExist+ " ;  Im On Site  :  "  +ImOnSite);
        ModuleMeasureAllTooltip();
        if (!ImOnSite)
            WarpingSlow(messageText, "warp");
        FlashWindowProbes();
        Host.Delay(3211);
        if (!ReadyForManeuverNot && !Tethering && ImOnSite)
            InBeltMineStep();
    }
    if (!OldSiteExist && activeshipnameSalvage)
    {
        Messages("                #  Old Site value  : "+OldSiteExist+ " .  Docking in salvage ship");
        WarpingSlow(RetreatBookmark, "dock");
    }
}

void LaunchMtuforSelf ()
{
    EnsureWindowInventoryOpen();
    EnsureWindowInventoryOpenActiveShip();
    var InventoryListItem = WindowInventory?.SelectedRightInventory?.ListView?.Entry?.ToArray();
    var MtuItem = WindowInventory?.SelectedRightInventory?.ListView?.Entry?.FirstOrDefault()?.LabelText?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(MTUName) ?? false);
    if (InventoryListItem?.FirstOrDefault()?.IsSelected ?? false )
        Sanderling.MouseClickLeft(MtuItem);
    ClickMenuEntryOnMenuRoot(WindowInventory?.SelectedRightInventory?.ListView?.Entry?.FirstOrDefault(), "Launch for Self");
    Messages("      Mtu Launched!!");
       Host.Delay(1211);
}

void RecoverMtu ()
{
     while (listOverviewMtu?.Length >0 )
    {
        MeChangeSalvage = false;
        Host.Delay(1111);
        Messages("      Recovering Mtu.");
        ClickMenuEntryOnMenuRoot(listOverviewMtu?.FirstOrDefault(), "Scoop to Cargo Hold");
        Host.Delay(1111);
    }
}
string KaboonusMidget ;
void KaboonusTalk()
{
    Host.Log("      Checking your wallet ....");
    Host.Delay(2111);
    var KaboonusWithTexturePathMatch = new Func<string, MemoryStruct.IUIElement>(texturePathRegexPattern =>
                    Measurement?.Neocom?.Button?.FirstOrDefault(candidate => candidate?.TexturePath?.RegexMatchSuccess(texturePathRegexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false));
    var KaboonusButton = KaboonusWithTexturePathMatch("wallet");
    if (KaboonusButton == null)
        return;
        Sanderling.MouseMove(KaboonusButton);
    for (int i = 3; i >= 0; i--)
        {
            KaboonusMidget = 	Measurement?.Tooltip?.FirstOrDefault()?.LabelText?.FirstOrDefault(entry =>entry.Text.RegexMatchSuccessIgnoreCase("\\d",RegexOptions.IgnoreCase))?.Text ;
    
            			if(!KaboonusMidget?.IsNullOrEmpty() ?? false)
				break;
            Sanderling.MouseMove(KaboonusButton);
            Sanderling.WaitForMeasurement();
            Sanderling.MouseMove(KaboonusButton);
        }
        var  Preparatus = Regex.Replace(KaboonusMidget, "[^0-9]+", "");
    Double.TryParse(Preparatus,out MagicalPrepare);
    Paracelsus = MagicalPrepare - HocusPocusPreparatus;
    Host.Delay(2111);
}


void InInventoryUnloadItems() => InInventoryUnloadItemsTo(UnloadDestContainerName);
void InInventoryUnloadItemsTo(string DestinationContainerName)
{
    Host.Log("               Unload items to '" + DestinationContainerName + "'.");
    EnsureWindowInventoryOpenActiveShip();
    var DestinationContainerLabelRegexPattern =
            InventoryContainerLabelRegexPatternFromContainerName(DestinationContainerName);
        var DestinationContainer =
            WindowInventory?.LeftTreeListEntry?.SelectMany(entry => new[] { entry }.Concat(entry.EnumerateChildNodeTransitive()))
            ?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(DestinationContainerLabelRegexPattern) ?? false);
    for (; ; )
    {
                EnsureWindowInventoryOpenActiveShip();
        Host.Delay(500);
        var oreHoldListItem = WindowInventory?.SelectedRightInventory?.ListView?.Entry?.ToArray();
        var oreHoldItem = oreHoldListItem?.FirstOrDefault();
        if (null == oreHoldItem)
            break;    //    0 items in Cargo
        if (1 < oreHoldListItem?.Length)
            ClickMenuEntryOnMenuRoot(oreHoldItem, @"select\s*all");
         if (null == DestinationContainer)
            Host.Log("               Houston, we have a problem: '" + DestinationContainerName + "' not found");
        Sanderling.MouseDragAndDrop(oreHoldItem, DestinationContainer);
            Host.Log("               Unloaded ALL items to '" + DestinationContainerName + "'.");
    }
        Host.Delay(1111);
        Sanderling.MouseClickLeft(DestinationContainer);
        ClickMenuEntryOnMenuRoot(WindowInventory?.SelectedRightInventory?.ListView?.Entry?.FirstOrDefault(), @"stack all");
        Host.Log("               Stack All in '" + UnloadDestContainerName+ "' .");
}

void RepairShop ()
{
    Host.Log("               repairing your stuff");

    Sanderling.MouseClickLeft(WindowInventory?.ActiveShipEntry);
    Host.Delay(500);
    ClickMenuEntryOnMenuRoot(WindowInventory?.ActiveShipEntry, "Get repair quote");
    Sanderling.WaitForMeasurement();

    var RepairShopWindow = Measurement?.WindowOther?.FirstOrDefault(w => (w?.Caption.RegexMatchSuccessIgnoreCase("Repairshop") ?? false));
    Sanderling.WaitForMeasurement();
    var RepairAllButton = Sanderling?.MemoryMeasurementParsed?.Value?.WindowOther?.FirstOrDefault()?.ButtonText?.FirstOrDefault(text => text.Text.RegexMatchSuccessIgnoreCase("repair all"));
    Sanderling.WaitForMeasurement();
    Host.Delay(1500);
	if (RepairAllButton !=null)
	{
        Sanderling.MouseClickLeft(RepairAllButton);
        Host.Delay(500);
    }
    Sanderling.WaitForMeasurement();
    if (RepairShopWindow?.HeaderButton != null)
    {
        if (RepairShopWindow !=null)
            Sanderling.MouseMove(RepairShopWindow.LabelText.FirstOrDefault());
        var closeButton = RepairShopWindow.HeaderButton?.FirstOrDefault(x => x.HintText == "Close");
        if (closeButton != null)
            Sanderling.MouseClickLeft(closeButton);
    }
}

void MoveDronesToFolder()
{
    while(DronesInBayFromFoldersCount != DronesInBayCount)
    {
        ClickMenuEntryOnPatternMenuRoot(AllDrones?.FirstOrDefault()?.LabelText?.Where(combatdronename =>combatdronename?.Text?.RegexMatchSuccessIgnoreCase(@LabelNameAttackDrones) ?? false)?.FirstOrDefault(), "Move Drone", CombatDronesFolder);
        Host.Delay(444);
        ClickMenuEntryOnPatternMenuRoot(AllDrones?.FirstOrDefault()?.LabelText?.Where(salvagedronename =>salvagedronename?.Text?.RegexMatchSuccessIgnoreCase(@LabelNameSalvageDrones) ?? false)?.FirstOrDefault(), "Move Drone", SalvageDronesFolder);
        Host.Delay(444);
        Host.Log("               moving a drone to his folder");
    }
}

var NoQuantity = true;

void Refill ()
{
    EnsureWindowInventoryOpen();
    EnsureWindowInventoryOpenActiveShip();

    if(InventoryActiveShipDronesContainer == null && !(IsExpanded(inventoryActiveShip) ?? false))
        Sanderling.MouseClickLeft(inventoryActiveShip?.ExpandToggleButton);
    Sanderling.WaitForMeasurement();
    Sanderling.MouseClickLeft(InventoryActiveShipDronesContainer);
        Sanderling.WaitForMeasurement();
        Host.Delay(3111);
    var HangarContainerLabelRegexPattern =
        InventoryContainerLabelRegexPatternFromContainerName(UnloadDestContainerName);
    var HangarContainer =
        WindowInventory?.LeftTreeListEntry?.SelectMany(entry => new[] { entry }.Concat(entry.EnumerateChildNodeTransitive()))
        ?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(HangarContainerLabelRegexPattern) ?? false);
    Host.Delay(1111);
        Host.Log("               Drones %   " +DronesHoldFillPercent+ "");
    if (DronesHoldFillPercent < 90)
    {
        Sanderling.MouseClickLeft(HangarContainer);
            Host.Delay(3111);
        NoQuantity = true;

        RefillGoods(LabelNameAttackDrones , InventoryActiveShipDronesContainer);
    }
     Sanderling.MouseClickLeft(HangarContainer);
    Host.Delay(1111);
    if (UseMissiles)
    {
        Sanderling.MouseClickLeft(HangarContainer);
            Host.Delay(3111);
        NoQuantity = false;
        reasonDrones = false;
        RefillGoods(MissilesName , InventoryActiveShipContainer);
    }
    Host.Delay(511);
    Sanderling.MouseClickLeft(HangarContainer);
        RefillGoods(MTUName , InventoryActiveShipContainer);
        Host.Delay(511);
    Sanderling.MouseClickLeft(HangarContainer);
}

void RefillGoods(string goody , IUIElement deposit)
{
    Sanderling.MouseClickLeft(WindowInventory?.InputText?.FirstOrDefault());
    Sanderling.TextEntry(goody);
        Host.Delay(1111);
    var RefillListItem = WindowInventory?.SelectedRightInventory?.ListView?.Entry?.ToArray();
    if (!RefillListItem?.IsNullOrEmpty() ?? false)
    Host.Log("              Filling with : " +goody);
    var RefillItem = RefillListItem?.FirstOrDefault();
    Sanderling.KeyDown(VirtualKeyCode.SHIFT);
        Host.Delay(511);
    Sanderling.MouseDragAndDrop(RefillItem , deposit);
        Host.Delay(511);
    Sanderling.KeyUp(VirtualKeyCode.SHIFT);
    Host.Delay(1511);
    if (UseMissiles && !NoQuantity)
        Sanderling.TextEntry(QuantityMissiles);
    Host.Delay(511);
    Sanderling.KeyboardPress(returnkey);
    NoQuantity = true;
    Sanderling.MouseClickLeft(WindowInventory?.SelectedRightFilterButtonClear);
    Host.Delay(811);
    Sanderling.MouseClickLeft(WindowInventory?.ActiveShipEntry);
}
void  ChangeToRatting ()
{
    EnsureWindowInventoryOpen();
    var RattingShip = WindowInventory?.LeftTreeListEntry?.SelectMany(entry => new[] { entry }.Concat(entry.EnumerateChildNodeTransitive()))
        ?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(RattingShipName) ?? false);
    if (!activeshipnameRatting)
    {
        ClickMenuEntryOnMenuRoot(RattingShip, "Make Active");
            Host.Delay(3111);
            
        Sanderling.MouseClickLeft(Measurement?.Neocom?.InventoryButton);
            Host.Delay(3111);
         SiteFinished = false;
             
    }
}

void  ChangeToSalvage ()
{
    EnsureWindowInventoryOpen();
    var SalvageShip = WindowInventory?.LeftTreeListEntry?.SelectMany(entry => new[] { entry }.Concat(entry.EnumerateChildNodeTransitive()))
        ?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(SalvageShipName) ?? false);
    if (!activeshipnameSalvage)
    {
        ClickMenuEntryOnMenuRoot(SalvageShip, "Make Active");
        Host.Delay(3111);
        MeChangeSalvage = false;
        EnsureWindowInventoryOpenActiveShip();
        Sanderling.WaitForMeasurement();
        MemoryUpdate();
        Sanderling.MouseClickLeft(Measurement?.Neocom?.InventoryButton);
        Host.Delay(3111);
    }
}


public string GetTempPath()
{
 string path = ".\\";
//string path = "D:\\ISKLOG";

    if (!path.EndsWith("\\")) path += "\\";
    return path;
}

public void LogMessageToFile(string msg)
{
    System.IO.StreamWriter sw = System.IO.File.AppendText(
        GetTempPath() +CharName+".txt");
    try
    {
        string logLine = System.String.Format(
            "{0:G}: {1}.", System.DateTime.Now, msg);
        sw.WriteLine(logLine);
    }
    finally
    {
        sw.Close();
    }
}


bool AnomalySuitableGeneral(MemoryStruct.IListEntry scanResult) =>
    scanResult?.CellValueFromColumnHeader(AnomalyToTakeColumnHeader)?.RegexMatchSuccessIgnoreCase(AnomalyToTake) ?? false;
bool ActuallyAnomaly(MemoryStruct.IListEntry scanResult) =>
       scanResult?.CellValueFromColumnHeader("Distance")?.RegexMatchSuccessIgnoreCase("km") ?? false;
bool IgnoreAnomaly(MemoryStruct.IListEntry scanResult) =>
!(scanResult?.CellValueFromColumnHeader(AnomalyToTakeColumnHeader)?.RegexMatchSuccessIgnoreCase(AnomalyToTake) ?? false);
bool IsEnemyBackgroundColor(ColorORGB color) =>
    color.OMilli == 500 && color.RMilli == 750 && color.GMilli == 0 && color.BMilli == 0;
bool IsFriendBackgroundColor(ColorORGB color) =>
    (color.OMilli == 500 && color.RMilli == 0 && color.GMilli == 150 && color.BMilli == 600) || (color.OMilli == 500 && color.RMilli == 100 && color.GMilli == 600 && color.BMilli == 100);
bool IsNeutralOrEnemy(IChatParticipantEntry participantEntry) =>
   !(participantEntry?.FlagIcon?.Any(flagIcon =>
     new[] { "good standing", "excellent standing", "Pilot is in your (fleet|corporation|alliance)", "Pilot is an ally in one or more of your wars", }
     .Any(goodStandingText =>
        flagIcon?.HintText?.RegexMatchSuccessIgnoreCase(goodStandingText) ?? false)) ?? false);


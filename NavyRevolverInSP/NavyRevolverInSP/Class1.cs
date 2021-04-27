using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using System.Windows.Forms;
using GTA.Native;
using GTA.Math;
using System.Drawing;

namespace NavyRevolverInSP
{
    public class Class1 : Script
    {
        public ScriptSettings Config;
        public bool firstTime = false;
        public string ModName = "Navy Revolver In SP";
        public string Developer = "HKH191";
        public string Version = "v1.0.0";
        public int Stage;
        public List<Prop> Props = new List<Prop>();
        public int LastClueLoc;
        public Vehicle Van;
        public bool CreatedVan;
        public Ped Slasher;
        public bool WaitingforRand;
        public bool CreatedSlasher;
        public int Waittime;
        public int Timer;
        public int CWatitime;
        public List<Vector3> SpawnRegionsBase = new List<Vector3>();
        public List<Vector3> SpawnRegions = new List<Vector3>();
        public Vector3 SpawnMarker;
        public bool ShowSpawnRegion=false;
        public Blip SpawnRegionBlip;
        public bool EnableSlasherBlip = true;
        public Class1()
        {
            Random R = new Random();
            LastClueLoc = R.Next(1, 5);
            Tick += onTick;
            Aborted += OnShutdown;
            Interval = 1;
            Setup();

            SpawnRegionsBase.Add(new Vector3(1492.6f,3148.2f,44));
            SpawnRegionsBase.Add(new Vector3(1984.7f,3039.8f,46));
            SpawnRegionsBase.Add(new Vector3(2394.34f, 3101.5f,48));
            SpawnRegionsBase.Add(new Vector3(2169.7f,3360.5f,46.46f));
            SpawnRegionsBase.Add(new Vector3(2503.46f,4116.53f,38));
            SpawnRegionsBase.Add(new Vector3(977.177f,3603.45f,35.9f));
            SpawnRegionsBase.Add(new Vector3(423.02f,3545.23f,34.7f));
            SpawnRegionsBase.Add(new Vector3(343.511f,3407.9f,38));
            SpawnRegionsBase.Add(new Vector3(59.69f,3704.9f,40));
            SpawnRegionsBase.Add(new Vector3(255.9f,3137.3f,42.0f));
            SpawnRegionsBase.Add(new Vector3(434.8f,2968.5f,41.8f));
            SpawnRegionsBase.Add(new Vector3(588.6f,2909.8f,42.6f));
            SpawnRegionsBase.Add(new Vector3(931.8f,3023.59f,39));
            SpawnRegionsBase.Add(new Vector3(931.8f, 3023.59f, 39));
            SpawnRegionsBase.Add(new Vector3(1723.644f,3252.9f,45.2f));
            SpawnRegionsBase.Add(new Vector3(1123.06f,2870.5f,39.3f));
            SpawnRegionsBase.Add(new Vector3(1222.9f,2704.6f,40.7f));
            SpawnRegionsBase.Add(new Vector3(595.3f,2705.3f,42));
            SpawnRegionsBase.Add(new Vector3(345.4f,2593.4f,44.4f));
            SpawnRegionsBase.Add(new Vector3(211.5f,2456.35f,58.5f));


            foreach(Vector3 V in SpawnRegionsBase)
            {
                SpawnRegions.Add(V);
                SpawnRegions.Add(V.Around(1000));
                SpawnRegions.Add(V.Around(900));
                SpawnRegions.Add(V.Around(800));
                SpawnRegions.Add(V.Around(700));
            }
        }
        public static void TextNotification(string avatar, string author, string title, string message)
        {
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET");
            Function.Call(Hash._SET_NOTIFICATION_TEXT_ENTRY, new InputArgument[]
            {
            "STRING"
            });
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, new InputArgument[]
            {
            message
            });
            int CurrentNotification = Function.Call<int>(Hash._SET_NOTIFICATION_MESSAGE, new InputArgument[]
            {
            avatar,
            avatar,
            true,
            0,
            title,
            author
            });
        }
        void Setup()
        {
            LoadIniFile("scripts\\NavyRevolver.ini");
            
        }
        public Model RequestModel(String Prop)
        {
            var model = new Model(Prop);
            model.Request(250);

            // Check the model is valid
            if (model.IsInCdImage && model.IsValid)
            {
                // Ensure the model is loaded before we try to create it in the world
                while (!model.IsLoaded) Script.Wait(50);
                return model;




            }

            // Mark the model as no longer needed to remove it from memory.

            model.MarkAsNoLongerNeeded();
            return model;
        }
        public Model RequestModel(VehicleHash Prop)
        {
            var model = new Model(Prop);
            model.Request(250);

            // Check the model is valid
            if (model.IsInCdImage && model.IsValid)
            {
                // Ensure the model is loaded before we try to create it in the world
                while (!model.IsLoaded) Script.Wait(50);
                return model;




            }

            // Mark the model as no longer needed to remove it from memory.

            model.MarkAsNoLongerNeeded();
            return model;
        }
        public Model RequestModel(PedHash Prop)
        {
            var model = new Model(Prop);
            model.Request(250);

            // Check the model is valid
            if (model.IsInCdImage && model.IsValid)
            {
                // Ensure the model is loaded before we try to create it in the world
                while (!model.IsLoaded) Script.Wait(50);
                return model;




            }

            // Mark the model as no longer needed to remove it from memory.

            model.MarkAsNoLongerNeeded();
            return model;
        }
        public void OnShutdown(object sender, EventArgs e)
        {
            var A_0 = true;
            if (A_0)
            {  
                if(Slasher!=null)
                {
                    Slasher.Delete();
                }
                if(SpawnRegionBlip!=null)
                {
                    SpawnRegionBlip.Remove();
                }
                if(Van!=null)
                {
                    Van.Delete();
                }
                foreach(Prop P in Props)
                {
                    if (P!=null)
                    {
                        P.Delete();
                    }
                }
            }
        }
       
        public void LoadIniFile(string iniName)
        {
            try
            {
                Config = ScriptSettings.Load(iniName);


                Stage = Config.GetValue<int>("Setup", "Stage", Stage);

                ShowSpawnRegion = Config.GetValue<bool>("Debug", "ShowSpawnRegion", ShowSpawnRegion);
                EnableSlasherBlip = Config.GetValue<bool>("Debug", "EnableSlasherBlip", EnableSlasherBlip);
            }
            catch (Exception e)
            {
                UI.Notify("~r~Error~w~: Config.ini Failed To Load.");
            }
        }
        public int GetHour()
        {
            return Function.Call<int>(Hash.GET_CLOCK_HOURS);
        }
        public void onTick(object sender, EventArgs e)
        {
            OnKeyDown();

            // Mod info


            // start your script here:
           

            if (firstTime==false)
            {
                Prop P = World.CreateProp(RequestModel("ch_prop_ch_serialkiller_01a"), new Vector3(-133.84f, 1912.56f, 197.7f), new Vector3(0, 0, -90), true, false);
                P.FreezePosition = true;
                Props.Add(P);
                 P = World.CreateProp(RequestModel("ch_prop_collectibles_limb_01a"), new Vector3(1111.448f,3143.8f,37.3f), new Vector3(120, 30, -150), true, false);
                P.FreezePosition = true;
                Props.Add(P);

                P = World.CreateProp(RequestModel("ch_prop_ch_bloodymachete_01a"), new Vector3(1904.137f,4912.712f,48.81f), new Vector3(180, 160,50), true, false);
                P.FreezePosition = true;
                Props.Add(P);


              
                P = World.CreateProp(RequestModel("ch_prop_ch_boodyhand_01a"), new Vector3(-679.38f, 5800.79f, 17.6f), new Vector3(0, 0, 156.49f), true, false);
                P.FreezePosition = true;
                Props.Add(P);
           
               
                UI.Notify("~y~" + ModName + "~w~ " + Version + " by ~g~" + Developer + "~w~ Loaded");

                firstTime = true;
                Random R = new Random();
                LastClueLoc = R.Next(1, 5);
               
                if (Stage == 4)
                {
                    CreatedVan = true;
                    if (LastClueLoc==1)
                    {
                        Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(2437.769f,5833.6f,58.5f), -146);
                        Van.IsPersistent = true;
                    }
                    if (LastClueLoc == 2)
                    {
                        Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(2899.925f,3653.065f,44.5f), -175);
                        Van.IsPersistent = true;
                    }
                    if (LastClueLoc == 3)
                    {
                        Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(2567.742f,1265.013f,44.516f), 9);
                        Van.IsPersistent = true;
                    }
                    if (LastClueLoc == 4)
                    {
                        Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(-1701.851f,2617.66f,2.41f),-47);
                        Van.IsPersistent = true;
                    }
                    if (LastClueLoc == 5)
                    {
                        Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(-1570.53f,4421.54f,6.6f), 155);
                        Van.IsPersistent = true;

                    }
                    if (Van != null)
                    {
                        Script.Wait(100);
                        Van.PrimaryColor = VehicleColor.MetallicBlack;
                        Van.SecondaryColor = VehicleColor.MetallicBlack;
                        Van.OpenDoor(VehicleDoor.BackRightDoor,true,true);
                        Script.Wait(5);
                        Van.OpenDoor(VehicleDoor.BackLeftDoor, true, true);
                      

                      //  Game.Player.Character.Position = Van.Position;
                        Vector3 V = Van.GetOffsetInWorldCoords(new Vector3(0, -1.3f, -0.25f));
                        P = World.CreateProp(RequestModel("ch_prop_collectibles_garbage_01a"),V, Van.Rotation, true, false);
                        P.HasCollision = false;
                        Props.Add(P);
                       // UI.Notify("P " +Props.Count);
                    }
                  
                 }
             }
            if(Van!=null)
            {
               
                Vector3 V = Van.GetOffsetInWorldCoords(new Vector3(0, -1.3f, -0.25f));
               // GTA.World.DrawMarker(MarkerType.DebugSphere, V, Vector3.Zero, Vector3.Zero, new Vector3(0.15f, 0.15f, 0.15f), Color.Blue);
               if(Props.Count==5)
                {
                    if (Props[4] != null)
                    {
                        Props[4].Position = V;
                        Props[4].Rotation = Van.Rotation;
                    }
                }
                
            }
            if(Stage==0)
            {
                if(World.GetDistance(Game.Player.Character.Position,Props[0].Position) <3)
                {
                    DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to investigate the clue");
                    if (Game.IsControlJustPressed(2, GTA.Control.Context))
                    {
                        LoadIniFile("scripts\\NavyRevolver.ini");

                        Stage = 1; 
                        Config.SetValue<int>("Setup", "Stage", Stage);
                        Config.Save();
                        UI.Notify("Clues Investigated 1/5");
                    }
                }
            }
            if (Stage == 1)
            {
                if (World.GetDistance(Game.Player.Character.Position, Props[1].Position) < 3)
                {
                    DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to investigate the clue");
                    if (Game.IsControlJustPressed(2, GTA.Control.Context))
                    {
                        LoadIniFile("scripts\\NavyRevolver.ini");

                        Stage = 2;
                        Config.SetValue<int>("Setup", "Stage", Stage);
                        Config.Save();
                        UI.Notify("Clues Investigated 2/5");
                    }
                }
            }
            if (Stage == 2)
            {
                if (World.GetDistance(Game.Player.Character.Position, Props[2].Position) < 3)
                {
                    DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to investigate the clue");
                    if (Game.IsControlJustPressed(2, GTA.Control.Context))
                    {
                        LoadIniFile("scripts\\NavyRevolver.ini");

                        Stage = 3;
                        Config.SetValue<int>("Setup", "Stage", Stage);
                        Config.Save();
                        UI.Notify("Clues Investigated 3/5");
                    }
                }
            }
            if (Stage == 3)
            {
                if (World.GetDistance(Game.Player.Character.Position, Props[3].Position) < 3)
                {
                    DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to investigate the clue");
                    if (Game.IsControlJustPressed(2, GTA.Control.Context))
                    {
                        LoadIniFile("scripts\\NavyRevolver.ini");
                        if (CreatedVan == false)
                        {
                            CreatedVan = true;
                            #region Spawn Van
                            if (LastClueLoc == 1)
                            {
                                Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(2437.769f, 5833.6f, 58.5f), -146);
                                Van.IsPersistent = true;
                            }
                            if (LastClueLoc == 2)
                            {
                                Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(2899.925f, 3653.065f, 44.5f), -175);
                                Van.IsPersistent = true;
                            }
                            if (LastClueLoc == 3)
                            {
                                Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(2567.742f, 1265.013f, 44.516f), 9);
                                Van.IsPersistent = true;
                            }
                            if (LastClueLoc == 4)
                            {
                                Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(-1701.851f, 2617.66f, 2.41f), -47);
                                Van.IsPersistent = true;
                            }
                            if (LastClueLoc == 5)
                            {
                                Van = World.CreateVehicle(RequestModel(VehicleHash.Burrito3), new Vector3(-1570.53f, 4421.54f, 6.6f), 155);
                                Van.IsPersistent = true;

                            }
                            if (Van != null)
                            {
                                Script.Wait(100);
                                Van.PrimaryColor = VehicleColor.MetallicBlack;
                                Van.SecondaryColor = VehicleColor.MetallicBlack;
                                Van.OpenDoor(VehicleDoor.BackRightDoor, true, true);
                                Script.Wait(5);
                                Van.OpenDoor(VehicleDoor.BackLeftDoor, true, true);


                             //   Game.Player.Character.Position = Van.Position;
                                Vector3 V = Van.GetOffsetInWorldCoords(new Vector3(0, -1.3f, -0.25f));
                               Prop P = World.CreateProp(RequestModel("ch_prop_collectibles_garbage_01a"), V, Van.Rotation, true, false);
                                P.HasCollision = false;
                                Props.Add(P);
                                // UI.Notify("P " +Props.Count);
                            }
                            #endregion
                        }
                        Stage = 4;
                        Config.SetValue<int>("Setup", "Stage", Stage);
                        Config.Save();
                        UI.Notify("Clues Investigated 4/5");
                    }
                }

            }
            if (Stage == 4)
            {
               
                if (Props.Count == 5)
                {
                    if (World.GetDistance(Game.Player.Character.Position, Props[4].Position) < 3)
                    {
                        DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to investigate the clue");
                        if (Game.IsControlJustPressed(2, GTA.Control.Context))
                        {
                            LoadIniFile("scripts\\NavyRevolver.ini");

                            Stage = 5;
                            Config.SetValue<int>("Setup", "Stage", Stage);
                            Config.Save();
                            UI.Notify("Clues Investigated 5/5");
                            Script.Wait(5000);
                            UI.Notify("I'm sick of you sticking your nose in where it doesn't belong! ");
                            Script.Wait(100);
                            UI.Notify("Now i'm not going to lie... what happens next... won't be nice for you. ");
                        }
                    }

                }
            }
            if (Stage == 5)
            {
                
                
                
                if ((GetHour() < 5) || (GetHour() > 20))
                {
                    
                    if (WaitingforRand == false)
                    {
                        if (SpawnRegionBlip != null)
                        {
                            SpawnRegionBlip.Remove();
                        }
                        Random R = new Random();
                        Waittime = R.Next(500, 1200);
                        WaitingforRand = true;
                       
                        CreatedSlasher = false;
                        SpawnMarker = SpawnRegionsBase[0];
                     }
                    
                    
               

                    if (CreatedSlasher == false)
                    {
                        if (CWatitime != Waittime)
                        {
                            if (Timer != 3)
                            {
                                Timer++;
                            }
                            if (Timer == 3)
                            {
                                Timer = 0;
                                CWatitime++;
                                //  DisplayHelpTextThisFrame("C " + CWatitime + " W " + Waittime);
                            }
                        }
                        if (CWatitime >= Waittime)
                        {
                            foreach (Vector3 V in SpawnRegions)
                            {
                                if (World.GetDistance(Game.Player.Character.Position, V) < World.GetDistance(Game.Player.Character.Position, SpawnMarker))
                                {
                                    SpawnMarker = V;
                                }
                            }
                            if (World.GetDistance(Game.Player.Character.Position, SpawnMarker) < 900)
                            {

                                CreatedSlasher = true;
                                if (Slasher != null)
                                {
                                    Slasher.Delete();
                                }
                                Slasher = World.CreatePed((Model)0x696BE0A9, Game.Player.Character.Position.Around(25));
                                Slasher.Armor = 100;
                                Slasher.Health = 300;
                                Slasher.CanWrithe = false;
                                Slasher.Weapons.Give(WeaponHash.Machete, 1, true, true);
                                Slasher.Task.FightAgainst(Game.Player.Character);

                                if (EnableSlasherBlip == true)
                                {
                                    Slasher.AddBlip();
                                    Slasher.CurrentBlip.Sprite = BlipSprite.Beast;
                                    Slasher.CurrentBlip.Color = BlipColor.Red;
                                    Slasher.CurrentBlip.Name = "Slasher";
                                }

                            }
                        }
                    }
                    if (CreatedSlasher == true)
                    {
                        if (Slasher != null)
                        {
                            if (Slasher.IsDead == true)
                            {
                                Game.Player.Character.Weapons.Give((WeaponHash)0x917F6C8C, 9999, true, true);
                                if (Slasher.CurrentBlip != null)
                                {
                                    Slasher.CurrentBlip.Remove();
                                }
                                Slasher = null;
                                if (Slasher != null)
                                {
                                    Slasher.Delete();
                                }
                                if (SpawnRegionBlip != null)
                                {
                                    SpawnRegionBlip.Remove();
                                }
                                CreatedSlasher = false;
                                WaitingforRand = false;
                                Stage = 0;
                                Config.SetValue<int>("Setup", "Stage", Stage);
                                Config.Save();
                                Audio.SetAudioFlag("LoadMPData", true);
                                Audio.PlaySoundFrontend("Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
                                Audio.SetAudioFlag("LoadMPData", false);
                                Game.Player.Money += 500000;
                                TextNotification("CHAR_LESTER", "Navy Revolver", "Weapon Unlock", "Your Maze Bank Account has been credited with $500000");

                            }
                        }
                    }
                }                
                else
                {
                    if (Slasher != null)
                    {
                        Slasher.Delete();
                    }
                    if (SpawnRegionBlip != null)
                    {
                        SpawnRegionBlip.Remove();
                    }
                    CreatedSlasher = false;
                    WaitingforRand = false;
                }

            }
        }
        public void SetPass()
        {
         
        }
        void DisplayHelpTextThisFrame(string text)
        {
            InputArgument[] arguments = new InputArgument[] { "STRING" };
            Function.Call(Hash._0x8509B634FBE7DA11, arguments);
            InputArgument[] argumentArray2 = new InputArgument[] { text };
            Function.Call(Hash._0x6C188BE134E074AA, argumentArray2);
            InputArgument[] argumentArray3 = new InputArgument[] { 0, 0, 1, -1 };
            Function.Call(Hash._0x238FFE5C7B0498A6, argumentArray3);
        }
        public void OnKeyDown()
        {





        }
    }
}

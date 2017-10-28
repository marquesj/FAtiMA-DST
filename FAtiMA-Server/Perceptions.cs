﻿using Newtonsoft.Json;
using RolePlayCharacter;
using System.Collections.Generic;
using WellFormedNames;

namespace FAtiMA_Server
{
    public class Entity
    {
        public int GUID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Entity(int GUID/*, int x, int y, int z*/)
        {
            this.GUID = GUID;
            //X = x;
            //Y = y;
            //Z = z;
        }

        public override string ToString()
        {
            return "Entity: " + this.GUID;
        }
    }

    public class Item : Entity
    {
        public string Prefab { get; set; }
        public int Count { get; set; }

        public Item(int GUID, string prefab, int count/*, int x, int y, int z*/) : base(GUID/*, x, y, z*/)
        {
            this.Prefab = prefab;
            this.Count = count;
        }

        public void UpdateBelief(RolePlayCharacterAsset rpc)
        {
            rpc.UpdateBelief("Item(" + GUID + "," + Prefab /*+ "," + X + "," + Y + "," + Z */+ ")", Count.ToString());
        }

        public override string ToString()
        {
            return Count + " x " + Prefab;
        }
    }

    public class EquippedItems : Item
    {
        public string Slot { get; set; }

        public EquippedItems(int GUID, string prefab, int count, string slot/*, int x, int y, int z*/) : base(GUID, prefab, count/*, x, y, z*/)
        {
            this.Slot = slot;
        }

        public override string ToString()
        {
            return Slot + ": " + Prefab;
        }
    }

    public class Perceptions
    {
        List<Item> Vision { get; set; }
        List<Item> ItemSlots { get; set; }
        List<EquippedItems> EquipSlots { get; set; }
        public int Hunger { get; set; }
        public int Sanity { get; set; }
        public int Health { get; set; }
        public int Moisture { get; set; }
        public int Temperature { get; set; }
        public bool IsFreezing { get; set; }
        public bool IsOverheating { get; set; }

        //TODO: Add something to track which part of the day the agent is in.

        [JsonConstructor]
        public Perceptions(List<EquippedItems> EquipSlots, List<Item> Vision, List<Item> ItemSlots,
            float Hunger, float Sanity, float Health, float Moisture, float Temperature, bool IsFreezing, bool IsOverheating)
        {
            this.Vision = Vision;
            this.ItemSlots = ItemSlots;
            this.EquipSlots = EquipSlots;
            this.Hunger = (int) Hunger;
            this.Health = (int) Health;
            this.Sanity = (int) Sanity;
            this.Moisture = (int) Moisture;
            this.Temperature = (int) Temperature;
            this.IsFreezing = IsFreezing;
            this.IsOverheating = IsOverheating;
        }
        
        public void UpdateBeliefs(RolePlayCharacterAsset rpc)
        {
            /*
             * Find every InSight, Inventory, and IsEquipped belief and set them to false
             * Eventually try and delete the beliefs (depending on performance).
             * */

            var subset = new List<SubstitutionSet>();
            subset.Add(new SubstitutionSet());

            var beliefs = rpc.m_kb.AskPossibleProperties((Name)"InSight([x])", (Name)"SELF", subset);
            //Console.WriteLine("Query returned " + beliefs.Count() + " InSight beliefs.");
            foreach(var b in beliefs)
            {
                foreach (var s in b.Item2)
                {
                    rpc.UpdateBelief("InSight(" + s[(Name)"[x]"] + ")", "FALSE");
                    //rpc.RemoveBelief("InSight(" + s[(Name)"[x]"] + ")", "SELF");
                }
            }

            beliefs = rpc.m_kb.AskPossibleProperties((Name)"InInventory([x])", (Name)"SELF", subset);
            //Console.WriteLine("Query returned " + beliefs.Count() + " InInventory beliefs.");
            foreach (var b in beliefs)
            {
                foreach (var s in b.Item2)
                {
                    rpc.UpdateBelief("InInventory(" + s[(Name)"[x]"] + ")", "FALSE");
                    //rpc.RemoveBelief("InInventory(" + s[(Name)"[x]"] + ")", "SELF");
                }
            }

            beliefs = rpc.m_kb.AskPossibleProperties((Name)"IsEquipped([x], [y])", (Name)"SELF", subset);
            //Console.WriteLine("Query returned " + beliefs.Count() + " IsEquipped beliefs.");
            foreach (var b in beliefs)
            {
                foreach (var s in b.Item2)
                {
                    rpc.UpdateBelief("IsEquipped(" + s[(Name)"[x]"] + ")", "FALSE");
                }
            }

            /*
             * Update the KB with the new beliefs
             * */
            rpc.UpdateBelief("Hunger(SELF)", this.Hunger.ToString());
            rpc.UpdateBelief("Health(SELF)", this.Health.ToString());
            rpc.UpdateBelief("Sanity(SELF)", this.Sanity.ToString());
            rpc.UpdateBelief("IsFreezing(SELF)", this.IsFreezing.ToString());
            rpc.UpdateBelief("IsOverheating(SELF)", this.IsOverheating.ToString());
            rpc.UpdateBelief("Moisture(SELF)", this.Moisture.ToString());
            rpc.UpdateBelief("Temperature(SELF)", this.Temperature.ToString());
            
            foreach (Item i in Vision)
            {
                rpc.UpdateBelief("InSight("+ i.GUID + ")", "TRUE");
                i.UpdateBelief(rpc);
            }

            foreach(Item i in ItemSlots)
            {
                rpc.UpdateBelief("InInventory(" + i.GUID + ")", "TRUE");
                i.UpdateBelief(rpc);
            }

            foreach(EquippedItems i in EquipSlots)
            {
                rpc.UpdateBelief("IsEquipped(" + i.GUID + "," + i.Slot + ")", "TRUE");
                i.UpdateBelief(rpc);
            }
        }
        
        /**
         * Just a quick way to show the Perceptions that we just got from the 'body'
         **/
        public override string ToString()
        {
            string s = "Perceptions:\n";
            s += "\tHunger: " + this.Hunger;
            s += "\tSanity: " + this.Sanity;
            s += "\tHealth: " + this.Health;
            s += "\n\tMoisture: " + this.Moisture;
            s += "\tTemperature: " + this.Temperature;
            s += "\tIsFreezing: " + this.IsFreezing;
            s += "\tIsOverheating: " + this.IsOverheating;
            s += "\n\tVision:\n";
            foreach (Item v in Vision)
            {
                s += "\t\t" + v.ToString() + "\n";
            }
            s += "\tItemSlots:\n";
            foreach (Item i in ItemSlots)
            {
                s += "\t\t" + i.ToString() + "\n";
            }
            s += "\tEquipSlots:\n";
            foreach (Item e in EquipSlots)
            {
                s += "\t\t" + e.ToString() + "\n";
            }
            return s;
        }
    }
}

/*
 * Old code that proccessed events
 * */
//class Perception
//{
//    private List<> subject { get; set; }
//    private string actionName { get; set; }
//    private string target { get; set; }
//    private string type { get; set; }

//    public Perception(string s, string a, string t, string type)
//    {
//        this.subject = s;
//        this.actionName = a;
//        this.target = t;
//        this.type = type;
//    }

//    private Name ToName()
//    {
//        switch (type)
//        {
//            case "actionend":
//                return ToActionEnd();
//            case "actionstart":
//                return ToActionStart();
//            case "propertychange":
//                return ToPropertyChange();
//            default:
//                throw new Exception("Type of action not recognised");
//        }
//    }

//    private Name ToActionEnd()
//    {
//        return EventHelper.ActionEnd(subject, actionName, target);
//    }

//    private Name ToActionStart()
//    {
//        return EventHelper.ActionStart(subject, actionName, target);
//    }

//    private Name ToPropertyChange()
//    {
//        return EventHelper.PropertyChange(subject, actionName, target);
//    }

//    public static Name FromJSON(string s)
//    {
//        return JsonConvert.DeserializeObject<Perception>(s).ToName();
//    }
//}

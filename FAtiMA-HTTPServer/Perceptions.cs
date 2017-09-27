﻿using Newtonsoft.Json;
using RolePlayCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WellFormedNames;

namespace FAtiMA_HTTPServer
{
    public class Item
    {
        public int GUID { get; set; }
        public string Prefab { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

        public Item(int GUID, string prefab, string name, int count)
        {
            this.GUID = GUID;
            this.Prefab = prefab;
            this.Name = name;
            this.Count = count;
        }

        public static Item FromJSON(string s)
        {
            return JsonConvert.DeserializeObject<Item>(s);
        }

        public override string ToString()
        {
            return Count + " x " + Prefab;
        }
    }
    public class EquippedItems : Item
    {
        public string Slot { get; set; }

        public EquippedItems(int GUID, string prefab, string name, int count, string slot) : base(GUID, prefab, name, count)
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
        public float Hunger { get; set; }
        public float Sanity { get; set; }
        public float Health { get; set; }
        public float Moisture { get; set; }
        public float Temperature { get; set; }
        public bool IsFreezing { get; set; }
        public bool IsOverheating { get; set; }

        //TODO: Add something to track wich part of the day the agent is in.

        [JsonConstructor]
        public Perceptions(List<EquippedItems> EquipSlots, List<Item> Vision, List<Item> ItemSlots, 
            float hunger, float sanity, float health, float moisture, float temperature, bool isFreezing, bool isOverheating)
        {
            this.Vision = Vision;
            this.ItemSlots = ItemSlots;
            this.EquipSlots = EquipSlots;
            this.Hunger = hunger;
            this.Health = health;
            this.Sanity = sanity;
            this.Moisture = moisture;
            this.Temperature = temperature;
            this.IsFreezing = isFreezing;
            this.IsOverheating = isOverheating;
        }

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


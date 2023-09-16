using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

namespace MapObjects
{
    public interface MapProp
    {
        int[] pos { get; set; }
        string type { get; set; }
    }


    [System.Serializable]
    public class ObjectPrefab
    {
        // Used as a return type for GetObject
        public GameObject prefab;
        public int rotation;
        public int offsetX;
        public int offsetY;
    }

    [System.Serializable]
    public class Wall
    {
        public string type;
        public int[] start;
        public int[] end;
    }

    [System.Serializable]
    public class Floor
    {
        public string type;
        public int[] start;
        public int[] end;
    }

    [System.Serializable]
    public class DoorAndWindow : MapProp
    {
        public int[] pos { get; set; }
        public string type { get; set; }
    }

    [System.Serializable]
    public class Furniture : MapProp
    {
        public int[] pos { get; set; }
        public string type { get; set; }
    }

    [System.Serializable]
    public class Utensil : MapProp
    {
        public int[] pos { get; set; }
        public string type { get; set; }
    }

    [System.Serializable]
    public class Electronic : MapProp
    {
        public int[] pos { get; set; }
        public string type { get; set; }
    }

    [System.Serializable]
    public class Goal : MapProp
    {
        public int[] pos { get; set; }
        public string type { get; set; }
    }

    [System.Serializable]
    public class Person : MapProp
    {
        public int[] pos { get; set; }
        public string type { get; set; }
    }

    [System.Serializable]
    public class Layers
    {
        public List<Wall> walls;
        public List<Floor> floors;
        public List<DoorAndWindow> door_and_windows;
        public List<Furniture> furniture;
        public List<Utensil> utensils;
        public List<Electronic> eletronics;
        public List<Goal> goals;
        public List<Person> persons;
    }

    [System.Serializable]
    public class Map
    {
        public int[] size;
        public Layers layers;

        public Map(
            int[] size,
            List<Wall> walls,
            List<Floor> floors,
            List<DoorAndWindow> door_and_windows,
            List<Furniture> furniture,
            List<Utensil> utensils,
            List<Electronic> eletronics,
            List<Goal> goals,
            List<Person> persons
        )
        {
            this.size = size;

            this.layers = new Layers();

            this.layers.walls = walls;
            this.layers.floors = floors;
            this.layers.door_and_windows = door_and_windows;
            this.layers.furniture = furniture;
            this.layers.utensils = utensils;
            this.layers.eletronics = eletronics;
            this.layers.goals = goals;
            this.layers.persons = persons;
        }

        public string mapRepresentationString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("Map Representation: \n");
            sb.Append("Size: " + this.size[0] + " " + this.size[1] + "\n");
            sb.Append("Layers: \n");

            sb.Append("Walls: \n");
            foreach (Wall wall in this.layers.walls)
                sb.Append("\tWall: " + wall.type + " " + wall.start[0] + " " + wall.start[1] + " " + wall.end[0] + " " + wall.end[1] + "\n");


            sb.Append("Floors: \n");
            foreach (Floor floor in this.layers.floors)
                sb.Append("\tFloor: " + floor.type + " " + floor.start[0] + " " + floor.start[1] + " " + floor.end[0] + " " + floor.end[1] + "\n");


            sb.Append("DoorAndWindows: \n");
            foreach (DoorAndWindow door_and_window in this.layers.door_and_windows)
                sb.Append("\tDoorAndWindow: " + door_and_window.type + " " + door_and_window.pos[0] + " " + door_and_window.pos[1] + "\n");


            sb.Append("Furniture: \n");
            foreach (Furniture furniture in this.layers.furniture)
                sb.Append("\tFurniture: " + furniture.type + " " + furniture.pos[0] + " " + furniture.pos[1] + "\n");


            sb.Append("Utensils: \n");
            foreach (Utensil utensil in this.layers.utensils)
                sb.Append("\tUtensil: " + utensil.type + " " + utensil.pos[0] + " " + utensil.pos[1] + "\n");


            sb.Append("Electronics: \n");
            foreach (Electronic electronic in this.layers.eletronics)
                sb.Append("\tElectronic: " + electronic.type + " " + electronic.pos[0] + " " + electronic.pos[1] + "\n");


            sb.Append("Goals: \n");
            foreach (Goal goal in this.layers.goals)
                sb.Append("\tGoal: " + goal.type + " " + goal.pos[0] + " " + goal.pos[1] + "\n");


            sb.Append("Persons: \n");
            foreach (Person person in this.layers.persons)
                sb.Append("\tPerson: " + person.type + " " + person.pos[0] + " " + person.pos[1] + "\n");

            return sb.ToString();
        }

    }
}
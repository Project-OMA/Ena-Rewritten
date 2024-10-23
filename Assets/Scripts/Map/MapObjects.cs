using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

namespace MapObjects
{
    public interface MapProp
    {
        public int[] getPos();
        public string getType();
    }


    [Serializable]
    public class ObjectPrefab
    {
        // Used as a return type for GetObject
        public string name;
        public GameObject prefab;
        public int rotation;
        public float offsetX;
        public float offsetY;
    }

    [Serializable]
    public class Wall
    {
        public string type;
        public int[] start;
        public int[] end;
    }

    [Serializable]
    public class Floor
    {
        public string type;
        public int[] start;
        public int[] end;
    }

    [Serializable]
    public class Ceiling
    {
        public string type;
        public int[] start;
        public int[] end;
    }

    [Serializable]
    public class DoorAndWindow : MapProp
    {
        public int[] pos;
        public string type;

        public int[] getPos()
        {
            return pos;
        }

        public string getType()
        {
            return type;
        }
    }

    [Serializable]
    public class Furniture : MapProp
    {
        public int[] pos;
        public string type;

        public int[] getPos()
        {
            return pos;
        }

        public string getType()
        {
            return type;
        }

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.pos != null)
            {
                sb.Append("Furniture: " + this.type + " " + this.pos[0] + " " + this.pos[1]);
            }
            else
            {
                sb.Append("Furniture: " + this.type + " null");
            }
            return sb.ToString();
        }
    }

    [Serializable]
    public class Utensil : MapProp
    {
        public int[] pos;
        public string type;

        public int[] getPos()
        {
            return pos;
        }

        public string getType()
        {
            return type;
        }

    }

    [Serializable]
    public class Electronic : MapProp
    {
        public int[] pos;
        public string type;

        public int[] getPos()
        {
            return pos;
        }

        public string getType()
        {
            return type;
        }
    }

    [Serializable]
    public class Goal : MapProp
    {
        public int[] pos;
        public string type;

        public int[] getPos()
        {
            return pos;
        }

        public string getType()
        {
            return type;
        }
    }

    [Serializable]
    public class Person: MapProp
    {
        public int[] pos;
        public string type;

        public int[] getPos()
        {
            return pos;
        }

        public string getType()
        {
            return type;
        }

        public string ToString()
        {
            return "Person: " + this.type + " " + this.pos[0] + " " + this.pos[1];
        }
    }

    [Serializable]
    public class Layers
    {
        public List<Wall> walls;
        public List<Floor> floors;
        public List<Ceiling> ceilings;
        public List<DoorAndWindow> door_and_windows;
        public List<Furniture> furniture;
        public List<Utensil> utensils;
        public List<Electronic> eletronics;
        public List<Goal> goals;
        public List<Person> persons;
    }

    [Serializable]
    public class Map
    {
        public int[] size;
        public Layers layers;

        public Map(
            int[] size,
            List<Wall> walls,
            List<Floor> floors,
            List<Ceiling> ceilings,
            List<DoorAndWindow> door_and_windows,
            List<Furniture> furniture,
            List<Utensil> utensils,
            List<Electronic> eletronics,
            List<Goal> goals,
            List<Person> persons
        )
        {
            this.size = size;

            this.layers = new Layers
            {
                walls = walls,
                floors = floors,
                ceilings = ceilings,
                door_and_windows = door_and_windows,
                furniture = furniture,
                utensils = utensils,
                eletronics = eletronics,
                goals = goals,
                persons = persons
            };
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


            sb.Append("Ceilings: \n");
            foreach (Ceiling ceiling in this.layers.ceilings)
                sb.Append("\tCeiling: " + ceiling.type + " " + ceiling.start[0] + " " + ceiling.start[1] + " " + ceiling.end[0] + " " + ceiling.end[1] + "\n");


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
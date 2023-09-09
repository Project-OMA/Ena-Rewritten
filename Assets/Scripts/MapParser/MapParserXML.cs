using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapObjects;
using System.Xml;

namespace MapParser
{
    public class MapParserXML : IMapParser
    {
        public string xmlData;
        private XMLParser xmlParser;
        private const string CanvasPath = "/root/canvas";

        private int[] size = new int[2];
        private List<Wall> walls = new List<Wall>();
        private List<Floor> floors = new List<Floor>();
        private List<DoorAndWindow> door_and_windows = new List<DoorAndWindow>();
        private List<Furniture> furniture = new List<Furniture>();
        private List<Utensil> utensils = new List<Utensil>();
        private List<Electronic> electronics = new List<Electronic>();
        private List<Goal> goals = new List<Goal>();
        private List<Person> persons = new List<Person>();

        private List<string> validLayers = new List<string>
        {
            "floor",
            "walls",
            "door_and_windows",
            "furniture",
            "eletronics",
            "utensils",
            "interactive_elements",
            "persons"
        };

        public MapParserXML(string xmlData)
        {
            this.xmlData = xmlData;
            xmlParser = new XMLParser(xmlData);
        }

        private void seekSize(int[] size)
        {
            XmlNode canvasNode = xmlParser.Fetch(CanvasPath);
            string x = canvasNode.Attributes["width"].Value;
            string y = canvasNode.Attributes["height"].Value;
            this.size[0] = int.Parse(x) / 32;
            this.size[1] = int.Parse(y) / 32;
        }

        private List<string> seekLayer(string layerName)
        {
            string name = layerName;

            // check if layerName is valid
            if (!validLayers.Contains(layerName))
            {
                throw new System.ArgumentException("Layer name is not valid");
            }

            XmlNode layerNode = xmlParser.Fetch("/root/layers/layer[@name='" + name + "']");

            // check if layer exists
            if (layerNode == null)
            {
                throw new System.ArgumentException("Layer does not exist");
            }

            // convert layer data to string array
            string[] layerData = layerNode.InnerText.Split(',');
            // trim all whitespaces
            for (int i = 0; i < layerData.Length; i++)
            {
                layerData[i] = layerData[i].Trim();
            }
            return new List<string>(layerData);
        }

        private void seekWall(List<Wall> walls)
        {
            string name = "walls";
            List<string> layerData = seekLayer(name);
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i] == "-1") continue;

                int x = i % size[0];
                int y = i / size[0];
                Wall wall = new()
                {
                    type = layerData[i],
                    start = new int[2] { x, y },
                    end = new int[2] { x, y }
                };
                walls.Add(wall);
            }
        }
        private void seekFloor(List<Floor> floors)
        {
            string name = "floor";
            List<string> layerData = seekLayer(name);
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i] == "-1") continue;
                int x = i % size[0];
                int y = i / size[0];
                Floor obj = new()
                {
                    type = layerData[i],
                    start = new int[2] { x, y },
                    end = new int[2] { x, y }
                };
                floors.Add(obj);
            }
        }
        private void seekDoorAndWindow(List<DoorAndWindow> door_and_windows)
        {
            string name = "door_and_windows";
            List<string> layerData = seekLayer(name);
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i] == "-1") continue;
                int x = i % size[0];
                int y = i / size[0];
                DoorAndWindow door_and_window = new()
                {
                    pos = new int[2] { x, y },
                    type = layerData[i]
                };
                door_and_windows.Add(door_and_window);
            }
        }
        private void seekFurniture(List<Furniture> furniture)
        {
            string name = "furniture";
            List<string> layerData = seekLayer(name);
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i] == "-1") continue;
                int x = i % size[0];
                int y = i / size[0];
                Furniture obj = new()
                {
                    pos = new int[2] { x, y },
                    type = layerData[i]
                };
                furniture.Add(obj);
            }
        }
        private void seekUtensil(List<Utensil> utensils)
        {
            string name = "utensils";
            List<string> layerData = seekLayer(name);
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i] == "-1") continue;
                int x = i % size[0];
                int y = i / size[0];
                Utensil utensil = new()
                {
                    pos = new int[2] { x, y },
                    type = layerData[i]
                };
                utensils.Add(utensil);
            }
        }
        private void seekElectronics(List<Electronic> electronics)
        {
            string name = "eletronics";
            List<string> layerData = seekLayer(name);
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i] == "-1") continue;
                int x = i % size[0];
                int y = i / size[0];
                Electronic electronic = new()
                {
                    pos = new int[2] { x, y },
                    type = layerData[i]
                };
                electronics.Add(electronic);
            }
        }
        private void seekGoals(List<Goal> goals)
        {
            string name = "interactive_elements";
            List<string> layerData = seekLayer(name);
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i] == "-1") continue;
                int x = i % size[0];
                int y = i / size[0];
                Goal goal = new()
                {
                    pos = new int[2] { x, y },
                    type = layerData[i]
                };
                goals.Add(goal);
            }
        }
        private void seekPerson(List<Person> persons)
        {
            string name = "persons";
            List<string> layerData = seekLayer(name);
            for (int i = 0; i < layerData.Count; i++)
            {
                if (layerData[i] == "-1") continue;
                int x = i % size[0];
                int y = i / size[0];
                Person person = new()
                {
                    pos = new int[2] { x, y },
                    type = layerData[i]
                };
                persons.Add(person);
            }
        }
        public Map ParseMap()
        {
            Debug.Log("Parsing map from XML");
            seekSize(size);
            seekWall(walls);
            seekFloor(floors);
            seekDoorAndWindow(door_and_windows);
            seekFurniture(furniture);
            seekUtensil(utensils);
            seekElectronics(electronics);
            seekGoals(goals);
            seekPerson(persons);

            return new Map(size, walls, floors, door_and_windows, furniture, utensils, electronics, goals, persons);
        }
    }
}

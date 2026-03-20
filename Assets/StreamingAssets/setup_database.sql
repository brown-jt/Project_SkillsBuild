CREATE TABLE IF NOT EXISTS inventory
(
  slot_index INTEGER PRIMARY KEY, -- slot in the inventory
  item_id TEXT NOT NULL,
  quantity INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS zones
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL
);

-- Quests will have the ID of the scriptable object in Unity for easy referencing
CREATE TABLE IF NOT EXISTS quests
(
  quest_id TEXT PRIMARY KEY NOT NULL,
  zone_id INTEGER NOT NULL,
  active BOOLEAN DEFAULT FALSE,
  completed BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (zone_id) REFERENCES zones(id)
);

-- Insertions into the zones these will always be the same
INSERT INTO zones VALUES (0, "Hub");
INSERT INTO zones VALUES (1, "Factory");
INSERT INTO zones VALUES (2, "Forest");
INSERT INTO zones VALUES (3, "Warehouse");
INSERT INTO zones VALUES (4, "Security");
INSERT INTO zones VALUES (5, "Museum");
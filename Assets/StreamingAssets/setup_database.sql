CREATE TABLE IF NOT EXISTS inventory
(
  slot_index INTEGER PRIMARY KEY, -- slot in the inventory
  item_id TEXT NOT NULL, -- ID of the scriptable object
  quantity INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS zones
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL
);

-- Insertions into the zones these will always be the same
INSERT INTO zones VALUES (0, "Hub");
INSERT INTO zones VALUES (1, "Factory");
INSERT INTO zones VALUES (2, "Forest");
INSERT INTO zones VALUES (3, "Warehouse");
INSERT INTO zones VALUES (4, "Security");
INSERT INTO zones VALUES (5, "Museum");

-- Quests will have the ID of the scriptable object in Unity for easy referencing
CREATE TABLE IF NOT EXISTS quests
(
  quest_id TEXT PRIMARY KEY NOT NULL,
  zone_id INTEGER NOT NULL,
  active BOOLEAN DEFAULT FALSE,
  completed BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (zone_id) REFERENCES zones(id)
);

-- To track quest objectives and their progress
CREATE TABLE IF NOT EXISTS quest_objectives
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  quest_id TEXT NOT NULL,
  objective_index INTEGER NOT NULL,
  current_amount INTEGER NOT NULL DEFAULT 0,
  is_complete BOOLEAN NOT NULL DEFAULT FALSE,
  FOREIGN KEY (quest_id) REFERENCES quests(quest_id),
  UNIQUE (quest_id, objective_index)
);

-- To cache AI responses for questions within a quest
CREATE TABLE IF NOT EXISTS question_hints
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  quest_id TEXT NOT NULL,
  question_index INTEGER NOT NULL,
  response TEXT NOT NULL,
  created_at TEXT NOT NULL,
  last_used_at TEXT NOT NULL,
  usage_count INTEGER DEFAULT 0,
  is_active BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (quest_id) REFERENCES quests(quest_id)
)
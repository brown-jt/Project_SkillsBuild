CREATE TABLE IF NOT EXISTS players 
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  level INTEGER DEFAULT 1,
  experience INTEGER DEFAULT 0
);

CREATE TABLE IF NOT EXISTS items
(
  id INTEGER PRIMARY KEY,
  name TEXT NOT NULL,
  description TEXT,
  max_stack INTEGER DEFAULT 1,                         -- Maximum qty in a stack
  value INTEGER DEFAULT 0                              -- Gold value of item
);

CREATE TABLE IF NOT EXISTS inventory
(
  id INTEGER PRIMARY KEY AUTOINCREMENT,                -- Allows for easy look-up and differentiation between multiple stacks of same item
  player_id INTEGER NOT NULL,
  item_id INTEGER NOT NULL,
  quantity INTEGER DEFAULT 1,
  slot_index INTEGER,                                  -- Slot in the inventory
  FOREIGN KEY (player_id) REFERENCES players(id),
  FOREIGN KEY (item_id) REFERENCES items(id)
);

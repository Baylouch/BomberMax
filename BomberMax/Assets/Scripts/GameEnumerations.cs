/* A script who contains every enumerations used in game.
 * 
 * I think it's nice to have a place to centralise it.
 * 
 * */

public enum GameBonusType { Bomb, Explosion, Speed, Invincibility };

public enum MovementDirection { None = -1, Up = 0, Down = 1, Left = 2, Right = 3 };

public enum BotPriority { None = -1, DestructibleBlock = 0, Bonus = 1, Enemy = 2, InDanger = 3 };

public enum BotDifficulty { Easy = 0, Medium = 1, Hard = 2, Pro = 3 };

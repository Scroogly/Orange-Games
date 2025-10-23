public class Player
{
    public int health;
    public (float x, float y) position;

    public Player(int hp)
    {
        health = hp;
    }

    public void Move(float newX, float newY)
    {
        position = (newX, newY);
    }

    public void Attack()
    {
        // This just triggers attack action — actual enemy hit handled in controller
        UnityEngine.Debug.Log("Player attacks!");
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}

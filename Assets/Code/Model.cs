using R3;

public class Model
{
    public int health;
    public float horizontalInput;
    public float horizontalSpeed;
    public bool isJumping;
    public float minJumpHeight;
    public float maxJumpHeight;
    public float jumpSpeed;
    public ReactiveProperty<bool> isDropping;
    public float platformDeactivationDuration;
    public float dropSpeed;
    public float playerY;

    public Model()
    {
        health = 5;
        minJumpHeight = 2;
        maxJumpHeight = 8;
        horizontalSpeed = 12;
        platformDeactivationDuration = 0.2f;
        isDropping = new();
        dropSpeed = 22F;
        jumpSpeed = 20F;
    }
}

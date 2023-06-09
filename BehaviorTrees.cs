// Define the BehaviorTreeStatus enum
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using EE;
using MonoGame.Extended.Sprites;
using System;

// Define the BehaviorTreeStatus enum
public enum BehaviorTreeStatus
{
    Running,
    Success,
    Failure
}

// Define the BehaviorTreeNode abstract class
public abstract class BehaviorTreeNode
{
    public abstract BehaviorTreeStatus Tick(BehaviorTreeData data);
}
public class BehaviorTreeData
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    public void Set<T>(string key, T value)
    {
        data[key] = value;
    }

    public T Get<T>(string key)
    {
        if (data.TryGetValue(key, out object value) && value is T typedValue)
        {
            return typedValue;
        }

        return default(T);
    }
}


// Define the CompositeNode class
public abstract class CompositeNode : BehaviorTreeNode
{
    protected List<BehaviorTreeNode> childNodes = new List<BehaviorTreeNode>();

    public void AddChild(BehaviorTreeNode node)
    {
        childNodes.Add(node);
    }
}

// Define the SequenceNode class
public class SequenceNode : CompositeNode
{
    public override BehaviorTreeStatus Tick(BehaviorTreeData data)
    {
        foreach (BehaviorTreeNode childNode in childNodes)
        {
            BehaviorTreeStatus childStatus = childNode.Tick(data);

            if (childStatus != BehaviorTreeStatus.Success)
            {
                // If any child node fails or is running, return its status
                return childStatus;
            }
        }

        // All child nodes succeeded, return Success
        return BehaviorTreeStatus.Success;
    }
}

// Define the ActionNode class
public class ActionNode : BehaviorTreeNode
{
    public override BehaviorTreeStatus Tick(BehaviorTreeData data)
    {
        // Implement the action logic here
        // Return Success, Failure, or Running based on the action's outcome
        

        return BehaviorTreeStatus.Success;
    }
}
public class MoveAction : ActionNode
{
    private CustomSprite sprite;
    private List<VegSprite> vegsprites;
    private int lastspawnedindex;
    private Random random;
    private Vector2 targetPosition;
    private float moveSpeed;

    public MoveAction(CustomSprite sprite, Vector2 targetPosition, float moveSpeed, Random random, List<VegSprite> vegsprites, int lastspawnedindex)
    {
        this.sprite = sprite;
        this.targetPosition = targetPosition;
        this.moveSpeed = random.Next(-1, 1); 
        this.random = random;
        this.vegsprites = vegsprites;
        this.lastspawnedindex = lastspawnedindex;
    }

    public override BehaviorTreeStatus Tick(BehaviorTreeData data)
    {
        // Retrieve the list of sprites from the behavior tree data
        List<VegSprite> vegsprites = data.Get<List<VegSprite>>("Sprites");

        // Determine the desired sprite from the list
        VegSprite targetSprite = vegsprites[lastspawnedindex];

        // Update the target position based on the desired sprite
        targetPosition = targetSprite.Position;

        // Calculate the direction from the current position to the target position
        Vector2 direction = Vector2.Normalize(targetPosition - sprite.Position);

        // Update the sprite's velocity to move in the desired direction
        sprite.Velocity = direction * moveSpeed;

        // Return Success to indicate that the movement action has been performed
        return BehaviorTreeStatus.Success;
    }
}

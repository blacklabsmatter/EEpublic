using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EE
{
    public class SpriteBehavior : Game1
    {
        public enum BehaviorTreeStatus
        {
            Success,
            Failure,
            Running
        }
        public abstract class BehaviorTreeNodeZ
        {
            public abstract Task<BehaviorTreeStatus> Tick();
        }

        public abstract class CompositeNode : BehaviorTreeNode
        {
            protected List<BehaviorTreeNode> childNodes = new List<BehaviorTreeNode>();

            public void AddChild(BehaviorTreeNode node)
            {
                childNodes.Add(node);
            }
        }

        public class SequenceNode : CompositeNode
        {
            public override async Task<BehaviorTreeStatus> Tick()
            {
                foreach (var node in childNodes)
                {
                    var status = await node.Tick();
                    if (status != BehaviorTreeStatus.Success)
                        return status;
                }
                return BehaviorTreeStatus.Success;
            }
        }

        public class ActionNode : BehaviorTreeNode
        {
            public override async Task<BehaviorTreeStatus> Tick()
            {
                // Implement the action logic here
                // Return the appropriate BehaviorTreeStatus based on the action result
                // Add a default return statement
                return BehaviorTreeStatus.Failure;
            }
        }
        public class RandomMovementNode : ActionNode
        {
            private float duration;
            private float elapsedTime;
            private Vector2 targetPosition;
            private float targetSpeed;


            public RandomMovementNode(float minDuration, float maxDuration)
            {
                duration = (float)(new Random().NextDouble() * (maxDuration - minDuration) + minDuration);
            }

            public override async Task<BehaviorTreeStatus> Tick()
            {
                if (elapsedTime >= duration)
                {
                    // Transition to circular movement after random duration
                    return BehaviorTreeStatus.Success;
                }

                // Perform random movement logic
                MoveRandomly();

                elapsedTime += (float)Game1.gameTime.ElapsedGameTime.TotalSeconds;
                return BehaviorTreeStatus.Running;
            }

            private void MoveRandomly(sprite.Velocity, GameTime gameTime, Random random)
            {
                sprite.Velocity = new Vector2((((float)random.NextDouble() * 2) - 1), (((float)random.NextDouble() * 2) - 1));
            }
        }

        public class BehaviorTree
        {
            private BehaviorTreeNode root;

            public BehaviorTree(BehaviorTreeNode rootNode)
            {
                root = rootNode;
            }

            public async Task<BehaviorTreeStatus> Tick()
            {
                return await root.Tick();
            }
        }

        public class NPCController
        {
            private BehaviorTree behaviorTree;

            public NPCController()
            {
                // Instantiate and assemble the behavior tree
                behaviorTree = new BehaviorTree(CreateBehaviorTreeRoot());
            }

            public void Update()
            {
                // Call the behavior tree's tick function
                behaviorTree.Tick();
            }

            private BehaviorTreeNode CreateBehaviorTreeRoot()
            {
                var randomMovementNode = new RandomMovementNode(3f, 10f);
                var circleMovementNode = new CircleMovementNode(3f, 10f, sprite.Origin, sprite.Radius);

                var sequenceNode = new SequenceNode();
                sequenceNode.AddChild(randomMovementNode);
                sequenceNode.AddChild(circleMovementNode);

                return sequenceNode;
            }

        }
    }
}

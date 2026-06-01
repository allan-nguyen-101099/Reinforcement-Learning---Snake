# Snake Game ML-Agents Training Guide

This guide walks you through training an AI agent to play Snake using Unity ML-Agents and Reinforcement Learning (PPO algorithm).

---

## 📋 Overview

- **Agent**: The snake learns to eat cherries and avoid obstacles/walls
- **Observations**: 11 values (danger ahead/left/right, current direction, cherry direction)
- **Actions**: 3 discrete actions (go straight, turn left, turn right)
- **Reward**: +1.0 for eating cherry, -1.0 for dying, ±0.01 per step for distance to cherry
- **Training Time**: ~30-60 minutes on a normal PC

---

## 🔧 Prerequisites

1. **Unity** — 2020.3 LTS or newer (2021.3+ recommended)
2. **Python 3.8+** — installed and in your system PATH
3. **Git** — for installing ML-Agents from source (optional)
4. **Visual Studio / IDE** — for editing C# scripts

Verify Python is installed:
```bash
python --version
```

---

## 📦 Installation Steps

### Step 1: Install ML-Agents Python Package

Open PowerShell or Git Bash and run:

```bash
pip install mlagents
```

Verify installation:
```bash
mlagents-learn --help
```

### Step 2: Install ML-Agents Unity Package

1. Open the Snake project in Unity
2. Go to **Window → TextureImporter → Package Manager**
3. Click **+ (Add)** → **Add package by name**
4. Enter: `com.unity.ml-agents`
5. Wait for import to complete
6. **Assets → ML-Agents → Examples** should now exist

---

## 🎮 Unity Component Setup

This is the most critical step. The SnakeAgent will not train without these components.

### On the Snake GameObject (same object with `Snake.cs`):

**1. Add SnakeAgent Component**
- Already created (`Assets/Scripts/SnakeAgent.cs`)
- Drag-and-drop or **Add Component → SnakeAgent**

**2. Add Behavior Parameters**
- **Add Component → Behavior Parameters**
- **Behavior Name**: `SnakeBehavior` (must match config file exactly)
- **Space Size**: `11` (11 observations)
- **Discrete Actions**:
  - Click **+** to add 1 discrete branch
  - Set branch size to `3` (3 actions: straight, left, right)
  - Leave **Vector Action** at 0

**3. Add Decision Requester**
- **Add Component → Decision Requester**
- **Decision Period**: `1` (make decision every frame)
- **Take Actions Between Decisions**: checked (default)

### Optional: On the Camera (for visualization)

- **Add Component → Monitor**
- Displays live stats during training (fps, reward, etc.)

---

## 🚀 Training Steps

### Step 1: Prepare the Project

Ensure your project structure looks like:
```
Reinforcement-Learning---Snake/
├── Assets/
│   ├── Scripts/
│   │   ├── Snake.cs
│   │   ├── Movement.cs
│   │   ├── SnakeAgent.cs        ✓ Required
│   │   ├── GameManager.cs
│   │   ├── CherryController.cs
│   │   └── ... (other scripts)
│   ├── Prefabs/
│   │   ├── Snake.prefab
│   │   ├── Cherry.prefab
│   │   └── Walls.prefab
│   └── Scenes/
│       └── SampleScene.unity
├── config/
│   └── snake_config.yaml         ✓ Required (in this folder)
└── ProjectSettings/
```

### Step 2: Open Terminal/PowerShell

Navigate to the project root:
```bash
cd "d:\Self\Code_allan\Reinforcement Learning\Reinforcement-Learning---Snake"
```

### Step 3: Start Training

Run the training command:
```bash
mlagents-learn config/snake_config.yaml --run-id=snake_v1
```

**What this does:**
- Reads training config from `config/snake_config.yaml`
- Creates a folder `results/snake_v1/` to save checkpoints and logs
- Waits for Unity to connect

### Step 4: Press Play in Unity

1. In the Unity Editor, click **Play** (or press Ctrl+P)
2. The console should show: `Connected to Unity environment. Starting training.`
3. Training begins immediately

**During training:**
- The game runs at high speed (no frame rate limit)
- The snake stumbles around initially, then gradually learns
- Episodes reset automatically after death
- Do NOT interact with the editor (don't pause/modify the scene)

---

## 📊 Monitoring Training

### Real-Time Monitoring (TensorBoard)

Open a **second** terminal/PowerShell window:
```bash
tensorboard --logdir results
```

Then open **http://localhost:6006** in your browser.

**Graphs you'll see:**
- **Cumulative Reward**: Should increase over time (good sign!)
- **Episode Length**: How long before dying. Should increase.
- **Policy Loss**: Should decrease or stabilize.

### Console Output

The terminal shows:
```
[INFO] Starting environment from /path/to/Unity...
[INFO] Connected to Unity environment. Starting training.
Step: 10000. Mean Reward: 0.25. Episodes: 42
Step: 20000. Mean Reward: 1.50. Episodes: 84
Step: 30000. Mean Reward: 3.20. Episodes: 125
...
```

**Healthy training** = Mean Reward increases over time.

---

## ⏸️ Stopping Training

### Graceful Stop (saves model checkpoint)

Press `Ctrl+C` in the terminal. Training will finish the current batch and save the model.

### Force Stop

Press Ctrl+C twice, or stop the Unity play session.

---

## 🧠 Using the Trained Model

After training completes, a neural network file is created:

```
results/snake_v1/
├── run_logs/          (TensorBoard logs)
└── SnakeBehavior.onnx (the trained model)
```

### Step 1: Copy the Model into Unity

1. Find `results/snake_v1/SnakeBehavior.onnx`
2. Copy it to: `Assets/Models/` (create the folder if needed)
3. Rename to: `SnakeBehavior.onnx`

### Step 2: Assign to Behavior Parameters

1. Select the Snake GameObject in the scene
2. In **Behavior Parameters** component:
   - Drag `Assets/Models/SnakeBehavior.onnx` into the **Model** field
   - Set **Behavior Type** to **Inference Only**
3. Save the scene

### Step 3: Test the Agent

1. Click **Play**
2. The snake should now play autonomously (controlled by the trained model)
3. It won't die immediately anymore — it's learned to avoid obstacles and chase cherries

---

## 🔄 Continuing Training

If the agent still has room to improve:

1. Ensure the current `SnakeBehavior.onnx` is assigned in Behavior Parameters
2. Change **Behavior Type** back to **Default**
3. Run training again with a new run ID:
   ```bash
   mlagents-learn config/snake_config.yaml --run-id=snake_v2
   ```
4. Press Play in Unity

This continues learning from the previous model (transfer learning).

---

## ⚙️ Tuning the Config

If training isn't working well, try these adjustments in `config/snake_config.yaml`:

### Agent Not Learning (Reward stays ~0)
- **Increase** `learning_rate` to `5.0e-4`
- **Increase** `beta` to `1.0e-2` (encourage exploration)
- **Check** that rewards are actually being given in `SnakeAgent.cs`

### Agent Learns but Gets Stuck (repeating same behavior)
- **Increase** `beta` to `1.0e-2`
- **Decrease** `epsilon` to `0.1` (allow bigger policy changes)
- **Increase** `buffer_size` to `4096` (see more diverse experiences)

### Training is Too Slow
- **Decrease** `batch_size` to `32` (faster iterations)
- **Decrease** `num_epoch` to `2` (less reprocessing)
- **Increase** `learning_rate` to `5.0e-4`

### Out of Memory Error
- **Decrease** `buffer_size` to `1024`
- **Decrease** `batch_size` to `32`
- **Decrease** `hidden_units` to `64`

---

## 🐛 Troubleshooting

### "Connected to Unity environment. Starting training." but then nothing happens

**Cause**: SnakeAgent is not properly configured.

**Fix**:
- Check Snake GameObject has `SnakeAgent` component
- Verify `Behavior Parameters` → **Behavior Name** = `SnakeBehavior` (case-sensitive!)
- Verify **Space Size** = `11`
- Verify **Discrete Branches** = `[3]`

### "No GameObjects with behavior name SnakeBehavior found"

**Cause**: Behavior name mismatch.

**Fix**:
- In `Behavior Parameters`, change "SnakeBehavior" to exactly match config
- OR edit `config/snake_config.yaml` to match Unity

### Reward stays at -1 (agent always dies immediately)

**Cause**: Agent logic is broken or observations are wrong.

**Fix**:
- Check `SnakeAgent.cs` is actually attached
- Verify `CollectObservations()` adds exactly 11 values
- Check that `Movement.SetAction()` is being called (add debug log)
- Verify obstacles and cherries spawn correctly

### "ModuleNotFoundError: No module named 'mlagents'"

**Cause**: Python installation issue.

**Fix**:
```bash
pip install --upgrade mlagents
```

### Training is very slow (less than 10k steps/min)

**Cause**: Unity is rendering graphics.

**Fix**: Disable rendering in Unity (optional):
- **Edit → Project Settings → Quality → V Sync Count** = Don't Sync
- Or run Unity in headless mode (advanced)

---

## 📈 Expected Training Progress

| Time | Mean Reward | Behavior |
|---|---|---|
| Start (0 steps) | ~-0.5 | Random, dies immediately |
| 100k steps | ~0.1 | Avoids some walls, rarely eats |
| 500k steps | ~1.5 | Eats cherries, navigates obstacles |
| 1M steps | ~3.0 | Consistently plays well |
| 2M steps | ~5.0+ | Expert play |

If you don't see improvement by 500k steps, something is wrong. Check troubleshooting above.

---

## 🎓 What Each Config Line Does

See `config/snake_config.yaml` — every parameter has an inline comment explaining its purpose.

Key settings for Snake:
- `gamma: 0.99` — Plan far ahead (important for Snake where cherries take many steps to reach)
- `batch_size: 64` — Good balance for small network
- `beta: 5.0e-3` — Gentle exploration (increase if agent loops)
- `epsilon: 0.2` — PPO's safety mechanism (don't change much)
- `max_steps: 2000000` — Usually enough. Raise if still improving at the end

---

## 🚀 Next Steps

1. **Run training** with default config
2. **Monitor** progress with TensorBoard
3. **Tune** config if needed based on results
4. **Export** trained model and test in-game
5. **Iterate** — continue training with new run IDs

Good luck! 🎮🤖


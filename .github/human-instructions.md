あなたは VSCode の GitHub Copilot Agent を利用して VRChat のワールドを作成する**人閒**です。

- 行動の主体はあなたにありません。積極的に Copilot Agent に任せます。
- あなたは Stable Diffusion WebUI (AUTOMATIC-1111) を利用して必要な画像を生成します。
  - 必要なプロンプトは Copilot Agent が提供します。画像を生成したら Unity にインポートしてください。
  - Skybox の生成には任意の Stable Diffusion モデルと、 LatentLabs360 LoRA 、および asymmetric-tiling-sd-webui 拡張機能を利用します。パラメーターは以下の通りです：
    - width: 2048
    - height: 1024
    - Sampling method: DPM++ 2M Karras
    - Asymmetric tiling:
      - Active: True
      - Tiling X: True
      - Tiling Y: False
      - Start Tiling from step N: 0
      - Stop tiling after step N: -1
-

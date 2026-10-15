$ErrorActionPreference = "Stop"

$python = Join-Path $env:LOCALAPPDATA "Programs\Python\Python312\python.exe"
if (-not (Test-Path $python)) {
  throw "CPython 3.12 was not found at $python. Install it for this Windows user first."
}

$venv = Join-Path $env:LOCALAPPDATA "DarkCoders\whisper-asr-venv"
$venvPython = Join-Path $venv "Scripts\python.exe"
if (-not (Test-Path $venvPython)) {
  & $python -m venv $venv
  if ($LASTEXITCODE -ne 0) { throw "Could not create the Whisper virtual environment." }
}

& $venvPython -m pip install --disable-pip-version-check -r (Join-Path $PSScriptRoot "requirements.txt")
if ($LASTEXITCODE -ne 0) { throw "Could not install the Whisper server dependencies." }

if (-not $env:WHISPER_MODEL) { $env:WHISPER_MODEL = "base.en" }
Write-Host "Starting local Whisper ASR on http://127.0.0.1:8000 (model: $env:WHISPER_MODEL)."
Write-Host "The first start downloads model weights; keep this window open while using voice input."
& $venvPython -m uvicorn server:app --app-dir $PSScriptRoot --host 127.0.0.1 --port 8000
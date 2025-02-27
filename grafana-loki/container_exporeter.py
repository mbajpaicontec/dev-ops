import os
from flask import Flask, Response
import subprocess

app = Flask(__name__)

# Function to generate metrics
def generate_metrics():
    stats = subprocess.check_output("docker stats --no-trunc --no-stream --format \"{{.Name}} {{.CPUPerc}} {{.MemPerc}}\"", shell=True).decode("utf-8")

    cpus = ""
    mems = ""
    for line in stats.splitlines():
        name, cpu, mem = line.split()
        cpus += f"container_cpu_percent{{container=\"{name}\"}} {cpu.rstrip('%')}\n"
        mems += f"container_mem_percent{{container=\"{name}\"}} {mem.rstrip('%')}\n"

    metrics = "# HELP container_cpu_percent The CPU% value from docker-stats.\n"
    metrics += "# TYPE container_cpu_percent gauge\n"
    metrics += cpus
    metrics += "# HELP container_mem_percent The MEM% value from docker-stats.\n"
    metrics += "# TYPE container_mem_percent gauge\n"
    metrics += mems
    return metrics

@app.route("/metrics")
def metrics():
    metrics = generate_metrics()
    return Response(metrics, mimetype="text/plain")

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=8142)

# Set up

1. Edit the list of Cron jobs

```bash
sudo crontab -e
```

2. Add a job at reboot to run the bash script

```bash
# add to cron file
@reboot cd /home/eba54/Desktop/wilding-pines-cv/ && bash wilding-pines.sh 2>&1 | tee wilding-pines-cron-output.log
```

The Cron output will now log to `wilding-pines-cron-output.log` in the repo.

## How to kill the program once it autostarts

You probably don't want to leave the program running indefinitely when you start the NUC. This is how you kill it. The program will handle the kill signal gracefully.

If you are running `capture_pine_data.py` manually, you can just use `Ctrl + C`.

- Shut down the NUC
- `sudo kill <PID>`, where the PID is given in the log file. sudo is needed as root owns the running job (cron started it)

## Produced file sizes

The minutes of storage were calculated with 90GB as that was what was available in the NUC.

| Frame rate | X Resolution | Y Resolution | Approx. size / min | Mins. of footage with 90GB of storage |
| :--------: | :----------: | :----------: | :----------------: | :----------------------------------:  |
| 6          | 640          | 480          | 312MB              | 270                                   |
| 6          | 848          | 480          | 397MB              | 226                                   |
| 30         | 640          | 480          | 1.8GB              | 50                                    |
| 30         | 848          | 480          | 2.4GB              | 37.5                                  |


## Troubleshooting
If some Python packages are not available to the Cron job. Try uninstaling them and the installing them back with `sudo -H pip3 install <package name>`
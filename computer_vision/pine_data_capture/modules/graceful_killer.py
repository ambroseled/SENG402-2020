import signal

class GracefulKiller:
    _should_kill_now: bool = False

    def __init__(self, logging):
        self.logging = logging

        # NOTE: trying to set up a listener for SIGKILL will make the OS
        # a n g e r y
        signal.signal(signal.SIGINT, self.exit_gracefully)
        signal.signal(signal.SIGTERM, self.exit_gracefully)
        signal.signal(signal.SIGHUP, self.exit_gracefully)
        signal.signal(signal.SIGINT, self.exit_gracefully)
        signal.signal(signal.SIGQUIT, self.exit_gracefully)
        signal.signal(signal.SIGILL, self.exit_gracefully)
        signal.signal(signal.SIGTRAP, self.exit_gracefully)
        signal.signal(signal.SIGABRT, self.exit_gracefully)
        signal.signal(signal.SIGBUS, self.exit_gracefully)
        signal.signal(signal.SIGFPE, self.exit_gracefully)
        signal.signal(signal.SIGUSR1, self.exit_gracefully)
        signal.signal(signal.SIGSEGV, self.exit_gracefully)
        signal.signal(signal.SIGUSR2, self.exit_gracefully)
        signal.signal(signal.SIGPIPE, self.exit_gracefully)
        signal.signal(signal.SIGALRM, self.exit_gracefully)
        signal.signal(signal.SIGTERM, self.exit_gracefully)
    
    def exit_gracefully(self, signum, frame):
        self._should_kill_now = True
        self.logging.info('Received signal to end process: {}'.format(signum))

    def request_program_exit(self, reason: str):
        self._should_kill_now = True
        self.logging.info('Received request to end process. Reason: {}'.format(reason))

    def should_kill_now(self) -> bool:
        return self._should_kill_now
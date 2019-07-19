# Unidevel.Raspberry.Gpio

Ultra simple library to manage Raspberry Pi GPIO pins. After several accidents with other libraries I decided to write my own as minimal as possible. Use indexers to set or get state, no extra types involved, no extra libraries needed. 

`
IGpio gpio = new FileGpio();

gpio[2] = true;
gpio[3] = false;

bool gpio4State = gpio[4];
bool gpio5State = gpio[5];
`

Because it uses file interface, this might not be ultra fast. However, in most cases it will be absolutely enough.

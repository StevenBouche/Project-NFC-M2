import React, { useState, useEffect } from 'react';
import { View, Text, TouchableOpacity, StyleSheet, TextInput, Button } from 'react-native';
import NfcManager, { NfcTech, Ndef } from 'react-native-nfc-manager';
import * as Mqtt from 'react-native-native-mqtt';
import { Buffer } from "buffer"
import { Picker } from '@react-native-picker/picker'

const topic = 'userevent';

export const Read = () => {

	const [text, setText] = useState('');
	const [client, setClient] = useState(null);
	const [mqttAdress, setMqttAdress] = useState('');
	const [isConnected, setIsConnected] = useState(false);
	const [storeId, setStoreId] = useState("Fnac");

	const handleConnect = () => {
		setClient(new Mqtt.Client(`tcp://${mqttAdress}:9000`));
	}

	useEffect(() => {
		if (client) {
			client.on(Mqtt.Event.Message, (topic, message) => {
				console.log('Mqtt Message:', topic, message.toString());
			});

			client.on(Mqtt.Event.Connect, () => {
				console.warn('MQTT Connect');
				client.subscribe([topic], [0]);
				setIsConnected(true);
			});

			client.on(Mqtt.Event.Error, (error) => {
				console.warn('MQTT Error:', error);
			});

			client.on(Mqtt.Event.Disconnect, (cause) => {
				console.log('MQTT Disconnect:', cause);
			});

			client.connect({
				clientId: 'user',
				cleanSession: true,
			}, err => { if (err) console.warn(err); });
		}
	}, [client]);


	async function readNdef() {
		try {
			// register for the NFC tag with NDEF in it
			console.warn('Wait tag');
			await NfcManager.requestTechnology(NfcTech.Ndef);
			// the resolved tag object will contain `ndefMessage` property
			const tag = await NfcManager.getTag();
			console.warn(`tag ${Ndef.text.decodePayload(tag.ndefMessage[0].payload)} detected in ${storeId} | send to MQTT`);

			setText(Ndef.text.decodePayload(tag.ndefMessage[0].payload));
			const msg = Buffer.from(`{"UserId":"${Ndef.text.decodePayload(tag.ndefMessage[0].payload)}",
			"StoreId":"${storeId}"}`
				, "utf-8");
			
			client.publish(topic, msg, 0);
		} catch (ex) {
			console.warn('Oops!', ex);
		} finally {
			// stop the nfc scanning
			NfcManager.cancelTechnologyRequest();
		}
	}


	return (
		<View style={styles.container}>
			<TextInput
				style={styles.input}
				onChangeText={setMqttAdress}
				value={mqttAdress}
				placeholder="Enter your mqtt adress"
			/>
			<Picker
				selectedValue={storeId}
				style={{ height: 50, width: 150 }}
				onValueChange={(itemValue, itemIndex) => setStoreId(itemValue)}
			>
				<Picker.Item label="Fnac" value="Fnac" />
				<Picker.Item label="Monoprix" value="Monoprix" />
				<Picker.Item label="Carrefour" value="Carrefour" />
				<Picker.Item label="Darty" value="Darty" />

			</Picker>
			<View style={styles.buttonContainer}>
				<Button
					disabled={!mqttAdress}
					title="Connect tp mqtt"
					onPress={handleConnect}
				/>
				<Button
					disabled={!isConnected}
					title="Scan Tag"
					onPress={readNdef}
				/>
			</View>
			{!!text && (<Text>{`identifiant: ${text}`}</Text>)}
		</View>
	);
}

const styles = StyleSheet.create({
	input: {
		height: 40,
		margin: 12,
		borderWidth: 1,
		padding: 10,
		width: '70%',
	},
	container: {
		flex: 1,
		backgroundColor: '#fff',
		alignItems: 'center',
		justifyContent: 'center',
	},
	buttonContainer: {
		flexDirection: 'column',
		justifyContent: 'space-around',
		alignContent: 'center',
		height: '15%',
	},
});


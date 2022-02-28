import { Button } from 'react-native';
import { StyleSheet, Text, View } from 'react-native';

export const HomeScreen = ({ navigation }) => {
	return (
		<View style={styles.container}>
			<View style={styles.fixToText}>
				<Button
					title="Read TAG"
					onPress={() =>
						navigation.navigate('ReadTag')
					}
				/>
				<Button
					title="Write TAG"
					onPress={() =>
						navigation.navigate('WriteTag')
					}
				/>
			</View>
		</View >
	);
};

const styles = StyleSheet.create({
	container: {
		flex: 1,
		backgroundColor: '#fff',
		justifyContent: 'center',
	},
	fixToText: {
		flexDirection: 'row',
		justifyContent: 'space-evenly',
	},
});
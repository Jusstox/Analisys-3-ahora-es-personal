<?php
include 'db_connect.php';

$json = $_POST['data'];
$data = json_decode($json, true);

if (isset($data['items'])) {
    $stmt = $conn->prepare("INSERT INTO Player_positions (GameRun_ID, Pos_x, Pos_y, Pos_z, Rotation_Y_Camera, Rotation_Y_Player, Timestamp) VALUES (?, ?, ?, ?, ?, ?, ?)");

    foreach ($data['items'] as $item) {
        $stmt->bind_param("iddddds", 
            $item['gameRunID'], 
            $item['x'], 
            $item['y'], 
            $item['z'], 
            $item['rotY_Cam'], 
            $item['rotY_Player'], 
            $item['timestamp']
        );
        $stmt->execute();
    }
    echo "Batch uploaded";
    $stmt->close();
} else {
    echo "No items found";
}
$conn->close();
?>
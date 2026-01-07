<?php
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

include 'db_connect.php';

if (!isset($_POST['type']) || !isset($_POST['x'])) {
    die("Error: Missing POST data. Make sure Unity is sending 'type', 'x', 'y', 'z'.");
}

$type = $_POST['type'];
$x = $_POST['x'];
$y = $_POST['y'];
$z = $_POST['z'];

$sql = "SELECT Enemies_ID FROM Enemies_Table WHERE Enemy_Type = '$type' AND ABS(StartPos_x - $x) < 0.1 AND ABS(StartPos_z - $z) < 0.1";
$result = $conn->query($sql);

if (!$result) {
    die("SELECT Error: " . $conn->error); 
}

if ($result->num_rows == 0) {
    $stmt = $conn->prepare("INSERT INTO Enemies_Table (Enemy_Type, StartPos_x, StartPos_y, StartPos_z) VALUES (?, ?, ?, ?)");
    
    if (!$stmt) {
        die("PREPARE Error: " . $conn->error);
    }

    $stmt->bind_param("sddd", $type, $x, $y, $z);
    
    if ($stmt->execute()) {
        echo "New enemy created successfully";
    } else {
        echo "EXECUTE Error: " . $stmt->error;
    }
    $stmt->close();
} else {
    echo "Enemy already exists, skipping.";
}

$conn->close();
?>
<?php
include 'db_connect.php';

$run_id = $_POST['run_id'];
$amount = $_POST['amount'];
$source = $_POST['source'];
$x = $_POST['x']; $y = $_POST['y']; $z = $_POST['z'];

$stmt = $conn->prepare("INSERT INTO Heal_Event (GameRun_ID, Amount_Healed, Source, Pos_x, Pos_y, Pos_z) VALUES (?, ?, ?, ?, ?, ?)");
$stmt->bind_param("iisddd", $run_id, $amount, $source, $x, $y, $z);

if ($stmt->execute()) echo "Heal recorded";
else echo "Error: " . $stmt->error;

$conn->close();
?>
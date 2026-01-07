<?php
include 'db_connect.php';

$run_id = $_POST['run_id'];
$enemy_id = isset($_POST['enemy_id']) && $_POST['enemy_id'] != "" ? $_POST['enemy_id'] : NULL;
$source = $_POST['source'];
$health = $_POST['health'];
$x = $_POST['x']; $y = $_POST['y']; $z = $_POST['z'];

$stmt = $conn->prepare("INSERT INTO PlayerHit_Event (GameRun_ID, Enemy_ID, Damage_Source, Remaining_Health, Pos_x, Pos_y, Pos_z) VALUES (?, ?, ?, ?, ?, ?, ?)");
$stmt->bind_param("iisdddd", $run_id, $enemy_id, $source, $health, $x, $y, $z);

if ($stmt->execute()) echo "Hit recorded";
else echo "Error: " . $stmt->error;

$conn->close();
?>
package cm.arcIGStest;

import java.io.IOException;

public class runAE {


    public static void main(String[] args) {

        try {
            getLayerNameFromGDB.GetLayerNameFromGDB("C:\\Users\\Administrator\\GdbData\\zjgbj20200923.gdb");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
